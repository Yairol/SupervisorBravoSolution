using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using SupervisorBravo.Web.Models.DTOs;
using SupervisorBravo.Web.ViewModels;
using System.Drawing;

namespace SupervisorBravo.Web.Controllers.System.Performances
{
    public class PerformanceController : Controller
    {
        private readonly IDixellRepository _dixellRepository;

        public PerformanceController(IDixellRepository dixellRepository)
        {
            _dixellRepository = dixellRepository;
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Performance(DixellReporteViewModel model)
        {
            if (model.StartDateReport == default && model.EndDateReport == default)
            {
                model.EndDateReport = DateTime.Today.AddHours(6);
                model.StartDateReport = DateTime.Today.AddHours(-6);
            }

            await _dixellRepository.BeginTransaction();

            // 👉 Usamos directamente la función agregada del repositorio
            var aggregates = await ((ITemperatureRepository)_dixellRepository)
                .GetTemperatureAggregates(model.StartDateReport.ToUniversalTime(), model.EndDateReport.ToUniversalTime());

            await _dixellRepository.CommitTransaction();

            // Convertimos los resultados a DixellReporteItem para la vista
            model.Reports = aggregates.Select(a => new DixellReporteItem
            {
                DixellName = a.DixellName,
                SetPoint = a.SetPoint,
                AvgTemperature = a.AvgTemperature,
                MinTemperature = a.MinTemperature,
                MaxTemperature = a.MaxTemperature,
                OffTime = a.OffTime,
                DisconnectTime = a.DisconnectTime,
                ControlTime = a.ControlTime,
                AvgControl = a.AvgControl,
                AvgOffTime = a.AvgOffTime,
                AvgDisconnectTime = a.AvgDisconnectTime,
                ModbusId = a.ModbusId // usamos el Id como ModbusId
            }).OrderBy(r => r.ModbusId).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(DixellReporteViewModel model)
        {
            // ⚠️ Importante: recalcular datos con la misma función
            await _dixellRepository.BeginTransaction();

            var aggregates = await ((ITemperatureRepository)_dixellRepository)
                .GetTemperatureAggregates(model.StartDateReport.ToUniversalTime(), model.EndDateReport.ToUniversalTime());

            await _dixellRepository.CommitTransaction();

            model.Reports = aggregates.Select(a => new DixellReporteItem
            {
                DixellName = a.DixellName,
                SetPoint = a.SetPoint,
                AvgTemperature = a.AvgTemperature,
                MinTemperature = a.MinTemperature,
                MaxTemperature = a.MaxTemperature,
                OffTime = a.OffTime,
                DisconnectTime = a.DisconnectTime,
                ControlTime = a.ControlTime,
                AvgControl = a.AvgControl,
                AvgOffTime = a.AvgOffTime,
                AvgDisconnectTime = a.AvgDisconnectTime,
                ModbusId = a.ModbusId
            }).OrderBy(r => r.ModbusId).ToList();

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Reporte");

            string[] headers =
            {
                "Dispositivo", "SP", "AVG", "Min", "Max",
                model.Room == "Refrigeracion" ? "%Cool" : "Electro Válvula",
                "Tiempo Desconectado(horas)/% del intervalo",
                "Tiempo Apagado(Off)(horas)/% del intervalo"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            int row = 2;
            foreach (var item in model.Reports)
            {
                worksheet.Cells[row, 1].Value = item.DixellName;
                worksheet.Cells[row, 2].Value = item.SetPoint;
                worksheet.Cells[row, 3].Value = item.AvgTemperature;
                worksheet.Cells[row, 4].Value = item.MinTemperature;
                worksheet.Cells[row, 5].Value = item.MaxTemperature;
                worksheet.Cells[row, 6].Value = $"{item.AvgControl:F1}%";
                worksheet.Cells[row, 7].Value = $"{item.DisconnectTime.TotalHours:F1}/{item.AvgDisconnectTime:F1}%";
                worksheet.Cells[row, 8].Value = $"{item.OffTime.TotalHours:F1}/{item.AvgOffTime:F1}%";

                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                row++;
            }

            worksheet.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"ReporteDixell_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [HttpGet]
        public async Task<IActionResult> Resumen()
        {
            await _dixellRepository.BeginTransaction();
            var dispositivos = await _dixellRepository.GetAllDixellsWithoutTemperatures<DixellXR>(); // suponiendo que tienes este método
            await _dixellRepository.CommitTransaction();

            var model = new ResumenViewModel
            {
                Devices = dispositivos.Select(d => new DeviceCheckViewModel
                {
                    Id = d.Id,
                    Name = d.RoomName,
                    Selected = false
                }).ToList(),
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Resumen(ResumenViewModel model)
        {
            var seleccionados = model.Devices.Where(d => d.Selected).ToList();
            if (!seleccionados.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un dispositivo.");
                return View(model);
            }

            var datos = await CalcularPromedios(model.StartDate, model.EndDate, seleccionados);

            var excelFile = GenerarResumenExcel(datos, model.StartDate, model.EndDate);

            var fileName = $"Resumen_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        private async Task<Dictionary<string, Dictionary<DateTime, double>>> CalcularPromedios(
    DateTime start, DateTime end, List<DeviceCheckViewModel> devices)
        {
            var result = new Dictionary<string, Dictionary<DateTime, double>>();

            await _dixellRepository.BeginTransaction();

            foreach (var device in devices)
            {
                var registros = await ((ITemperatureRepository)_dixellRepository)
                    .GetTemperaturesByDateRange( start.ToUniversalTime(), end.ToUniversalTime(), device.Id);

                var dailyData = registros
                    .GroupBy(r => r.MeasurementTime.Date)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Average(x => x.TemperatureMeasurement)
                    );

                result[device.Name] = dailyData;
            }

            await _dixellRepository.CommitTransaction();

            return result;
        }
        private byte[] GenerarResumenExcel(
          Dictionary<string, Dictionary<DateTime, double>> datos,
          DateTime start, DateTime end)
        {
            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Resumen");

            // Encabezado
            ws.Cells[1, 1].Value = "Cámaras";
            ws.Cells[1, 1].Style.Font.Bold = true;
            ws.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

            var dias = Enumerable.Range(0, (end.Date - start.Date).Days + 1)
                                 .Select(offset => start.Date.AddDays(offset))
                                 .ToList();

            for (int i = 0; i < dias.Count; i++)
            {
                var cell = ws.Cells[1, i + 2];
                cell.Value = dias[i].ToString("dd/MM");
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            // Filas de dispositivos
            int row = 2;
            foreach (var kvp in datos)
            {
                ws.Cells[row, 1].Value = kvp.Key;
                ws.Cells[row, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                for (int i = 0; i < dias.Count; i++)
                {
                    if (kvp.Value.TryGetValue(dias[i], out double avg))
                    {
                        ws.Cells[row, i + 2].Value = avg;
                        ws.Cells[row, i + 2].Style.Numberformat.Format = "0.00"; // 👈 dos decimales
                    }
                    else
                    {
                        ws.Cells[row, i + 2].Value = "-";
                    }

                    ws.Cells[row, i + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                row++;
            }

            ws.Cells.AutoFitColumns();
            return package.GetAsByteArray();
        }
    }
}
