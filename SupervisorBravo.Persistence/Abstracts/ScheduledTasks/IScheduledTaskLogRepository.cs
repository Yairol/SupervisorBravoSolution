using SupervisorBravo.Domain.Entities.Schedule;
using SupervisorBravo.Persistence.Helpers;


namespace SupervisorBravo.Persistence.Abstracts.ScheduledTasks
{
    public interface IScheduledTaskExecutionLogRepository : IRepository
    {
        Task<List<ScheduledTaskExecutionLog>> GetLogByTask(ScheduledTask task);
        Task<ScheduledTaskExecutionLog> AddLog(ScheduledTaskExecutionLog log);
        Task<List<ScheduledTaskExecutionLog>> GetRecentLogs(int maxCount = 20);
        Task<int> GetTotalLogCountAsync();
        Task<List<ScheduledTaskExecutionLog>> GetLogsPageAsync(int pageIndex, int pageSize);
        IQueryable<ScheduledTaskExecutionLog> QueryAllLogs();
        Task<List<string>> GetAllDeviceNamesAsync();
        Task<List<string>> GetAllOutcomesAsync();
        Task<List<string>> GetAllActionsAsync();
        Task<ScheduledTaskExecutionLog?> GetByIdAsync(Guid id);
        Task DeleteLogAsync(ScheduledTaskExecutionLog log);
        Task<List<LogExportRow>> GetFilteredLogsForExportAsync(LogExportFilter filters);

    }
}
