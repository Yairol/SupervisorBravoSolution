using SupervisorBravo.Domain.Entities.Schedule;
using SupervisorBravo.Persistence.Helpers;


namespace SupervisorBravo.Persistence.Abstracts.ScheduledTasks
{
    public interface IScheduledTaskExecutionLogRepository : IRepository
    {
        /// <summary>
        /// Obtiene todos los logs de una tarea
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        Task<List<ScheduledTaskExecutionLog>> GetLogByTask(ScheduledTask task);
        /// <summary>
        /// Agrega un log al soporte de datos
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        Task<ScheduledTaskExecutionLog> AddLog(ScheduledTaskExecutionLog log);
        /// <summary>
        /// Obtiene los logs recientes por defecto 20
        /// </summary>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        Task<List<ScheduledTaskExecutionLog>> GetRecentLogs(int maxCount = 20);
        /// <summary>
        /// Obtiene el contador de logs para paginacion
        /// </summary>
        /// <returns></returns>
        Task<int> GetTotalLogCountAsync();
        /// <summary>
        /// Obtiene la pagina para la paginacion de los logs
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<List<ScheduledTaskExecutionLog>> GetLogsPageAsync(int pageIndex, int pageSize);
        /// <summary>
        /// Enlista Logs
        /// </summary>
        /// <returns></returns>
        IQueryable<ScheduledTaskExecutionLog> QueryAllLogs();
        /// <summary>
        /// Obtiene los nombres de los dispositivos con logs
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetAllDeviceNamesAsync();
        /// <summary>
        /// Obtiene todas las salidas
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetAllOutcomesAsync();
        /// <summary>
        /// Obtiene todas las acciones
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetAllActionsAsync();
        /// <summary>
        /// Obtiene un log del soporte de datos por su Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ScheduledTaskExecutionLog?> GetByIdAsync(Guid id);
        Task DeleteLogAsync(ScheduledTaskExecutionLog log);
        Task<List<LogExportRow>> GetFilteredLogsForExportAsync(LogExportFilter filters);

    }
}
