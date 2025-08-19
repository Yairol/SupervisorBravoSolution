using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Schedule;
using SupervisorBravo.Persistence.Abstracts.ScheduledTasks;
using SupervisorBravo.Persistence.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IScheduledTaskExecutionLogRepository
    {
        public async Task<List<ScheduledTaskExecutionLog>> GetLogByTask(ScheduledTask task)
        {
            var ctx = EnsureContext();
            return await ctx.Set<ScheduledTaskExecutionLog>()
                .Where(log => log.TaskId == task.Id)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }

        public async Task<List<ScheduledTaskExecutionLog>> GetRecentLogs(int maxCount = 25)
        {
            var ctx = EnsureContext();
            return await ctx.Set<ScheduledTaskExecutionLog>()
                .OrderByDescending(log => log.Timestamp)
                .Take(maxCount)
                .ToListAsync();
        }

        public async Task<ScheduledTaskExecutionLog> AddLog(ScheduledTaskExecutionLog log)
        {
            var ctx = EnsureContext();
            await ctx.Set<ScheduledTaskExecutionLog>().AddAsync(log);
            await ctx.SaveChangesAsync();
            return log;
        }

        public async Task<List<ScheduledTaskExecutionLog>> GetLogsPageAsync(int pageIndex, int pageSize)
        {
            var ctx = EnsureContext();
            return await ctx.Set<ScheduledTaskExecutionLog>()
                .Include(log => log.Task)
                .ThenInclude(task => task.Device)
                .OrderByDescending(log => log.Timestamp)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalLogCountAsync()
        {
            var ctx = EnsureContext();
            return await ctx.Set<ScheduledTaskExecutionLog>().CountAsync();
        }

        public IQueryable<ScheduledTaskExecutionLog> QueryAllLogs()
        {
            var ctx = EnsureContext();
            return ctx.ScheduledTaskExecutionLogs.AsQueryable();
        }

        public async Task<List<string>> GetAllDeviceNamesAsync()
        {
            var ctx = EnsureContext();
            return await ctx.Set<DixellXR>()
                .Select(d => d.RoomName)
                .Distinct()
                .OrderBy(n => n)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllOutcomesAsync() =>
            await Task.Run(() => Enum.GetNames(typeof(ExecutionOutcome)).ToList());

        public async Task<List<string>> GetAllActionsAsync() =>
            await Task.Run(() => Enum.GetNames(typeof(ActionType)).ToList());


        public async Task<ScheduledTaskExecutionLog?> GetByIdAsync(Guid id)
        {
            var ctx = EnsureContext();
            return await ctx.Set<ScheduledTaskExecutionLog>()
                .FirstOrDefaultAsync(log => log.Id == id);
        }

        public async Task DeleteLogAsync(ScheduledTaskExecutionLog log)
        {
            var ctx = EnsureContext();
            ctx.Set<ScheduledTaskExecutionLog>().Remove(log);
            await ctx.SaveChangesAsync();
        }

        public async Task<List<LogExportRow>> GetFilteredLogsForExportAsync(LogExportFilter filters)
        {
            var ctx = EnsureContext();
            var query = ctx.ScheduledTaskExecutionLogs
                .Include(l => l.Task).ThenInclude(t => t.Device)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Outcome) &&
                Enum.TryParse<ExecutionOutcome>(filters.Outcome, out var outcome))
            {
                query = query.Where(l => l.Outcome == outcome);
            }

            if (!string.IsNullOrWhiteSpace(filters.Action) &&
                Enum.TryParse<ActionType>(filters.Action, out var action))
            {
                query = query.Where(l => l.Task.Action == action);
            }

            if (!string.IsNullOrWhiteSpace(filters.DeviceName))
            {
                query = query.Where(l => l.Task.Device.RoomName == filters.DeviceName);
            }

            if (filters.FromUtc.HasValue)
            {
                query = query.Where(l => l.Timestamp >= filters.FromUtc.Value);
            }

            if (filters.ToUtc.HasValue)
            {
                query = query.Where(l => l.Timestamp <= filters.ToUtc.Value);
            }

            return await query
                .OrderByDescending(l => l.Timestamp)
                .Select(l => new LogExportRow
                {
                    Fecha = l.Timestamp.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"),
                    Acción = l.Task.Action.ToString(),
                    Dispositivo = l.Task.Device.RoomName ?? "(sin nombre)",
                    Resultado = l.Outcome.ToString(),
                    Mensaje = l.Message
                })
                .ToListAsync();
        }
    }
}
