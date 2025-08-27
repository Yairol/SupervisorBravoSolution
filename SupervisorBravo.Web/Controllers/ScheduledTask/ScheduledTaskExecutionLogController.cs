using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Schedule;
using SupervisorBravo.Persistence.Abstracts.ScheduledTasks;
using SupervisorBravo.Persistence.Helpers;
using SupervisorBravo.Web.ViewModels;

namespace SupervisorBravo.Web.Controllers
{
    public class ScheduledTaskLogController : Controller
    {
        private readonly IScheduledTaskExecutionLogRepository _logRepository;

        public ScheduledTaskLogController(IScheduledTaskExecutionLogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<IActionResult> Index(
            int page = 1,
            string? outcome = null,
            string? action = null,
            string? deviceName = null)
        {
            await _logRepository.BeginTransaction();
            const int pageSize = 25;

            var logsQuery = _logRepository.QueryAllLogs(); // usa IQueryable para filtros

            // ✅ Solución: evitar ToString() para que EF pueda traducir correctamente
            if (!string.IsNullOrWhiteSpace(outcome) &&
                Enum.TryParse(outcome, out ExecutionOutcome parsedOutcome))
            {
                logsQuery = logsQuery.Where(l => l.Outcome == parsedOutcome);
            }

            if (!string.IsNullOrWhiteSpace(action) &&
                Enum.TryParse(action, out ActionType parsedAction))
            {
                logsQuery = logsQuery.Where(l => l.Task.Action == parsedAction);
            }

            if (!string.IsNullOrWhiteSpace(deviceName))
            {
                logsQuery = logsQuery.Where(l => l.Task.Device.RoomName == deviceName);
            }

            var totalCount = await logsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var currentPage = Math.Clamp(page, 1, Math.Max(1, totalPages));

            var logs = await logsQuery
                .Include(l => l.Task).ThenInclude(t => t.Device)
                .OrderByDescending(l => l.Timestamp)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedLogs = logs.Select(log => new ScheduledTaskLogViewModel
            {
                Id = log.Id,
                Timestamp = log.Timestamp,
                Action = log.Task?.Action.ToString() ?? "(sin acción)",
                Outcome = log.Outcome.ToString(),
                RoomName = log.Task?.Device?.RoomName ?? "(sin nombre)",
                Message = log.Message
            }).ToList();

            var allActions = await _logRepository.GetAllActionsAsync();
            var allOutcomes = await _logRepository.GetAllOutcomesAsync();
            var allDeviceNames = await _logRepository.GetAllDeviceNamesAsync();

            var viewModel = new ScheduledTaskLogFilterViewModel
            {
                Logs = mappedLogs,
                SelectedOutcome = outcome,
                SelectedAction = action,
                SelectedDeviceName = deviceName,
                AvailableActions = allActions,
                AvailableOutcomes = allOutcomes,
                AvailableDeviceNames = allDeviceNames,
                CurrentPage = currentPage,
                TotalPages = totalPages
            };

            await _logRepository.CommitTransaction();
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _logRepository.BeginTransaction();

                var log = await _logRepository.GetByIdAsync(id);
                if (log == null)
                {
                    TempData["Error"] = "❌ El registro no existe.";
                    return RedirectToAction(nameof(Index));
                }

                await _logRepository.DeleteLogAsync(log);

                await _logRepository.CommitTransaction();

                TempData["Success"] = "✅ Registro eliminado correctamente.";
            }
            catch (Exception ex)
            {
                await _logRepository.RollbackTransaction();
                TempData["Error"] = $"❌ Error eliminando registro: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePageLogs(string logIds)
        {
            try
            {
                var ids = logIds.Split(',').Select(id => Guid.Parse(id)).ToList();

                await _logRepository.BeginTransaction();

                foreach (var id in ids)
                {
                    var log = await _logRepository.GetByIdAsync(id);
                    if (log != null)
                    {
                        await _logRepository.DeleteLogAsync(log);
                    }
                }

                await _logRepository.CommitTransaction();

                TempData["Success"] = "✅ Todos los registros de la página fueron eliminados.";
            }
            catch (Exception ex)
            {
                await _logRepository.RollbackTransaction();
                TempData["Error"] = $"❌ Error al eliminar los registros: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Export()
        {
            await _logRepository.BeginTransaction();
            var vm = new LogExportFilterViewModel
            {
                AvailableActions = await _logRepository.GetAllActionsAsync(),
                AvailableOutcomes = await _logRepository.GetAllOutcomesAsync(),
                AvailableDeviceNames = await _logRepository.GetAllDeviceNamesAsync()
            };
            await _logRepository.CommitTransaction();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(LogExportFilterViewModel filters)
        {
            await _logRepository.BeginTransaction();
            var filter = new LogExportFilter
            {
                Outcome = filters.SelectedOutcome,
                Action = filters.SelectedAction,
                DeviceName = filters.SelectedDeviceName,
                FromUtc = filters.FromDate?.ToUniversalTime(),
                ToUtc = filters.ToDate?.ToUniversalTime()
            };

            var logs = await _logRepository.GetFilteredLogsForExportAsync(filter);

            using var package = new OfficeOpenXml.ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Logs");

            ws.Cells.LoadFromCollection(logs, true);

            var fileBytes = package.GetAsByteArray();
            await _logRepository.CommitTransaction();

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"LogsEjecutados_{timestamp}.xlsx";

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


    }
}
