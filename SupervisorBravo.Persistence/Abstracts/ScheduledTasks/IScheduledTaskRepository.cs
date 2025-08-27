using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Schedule;

namespace SupervisorBravo.Persistence.Abstracts.ScheduledTasks
{
    public interface IScheduledTaskRepository : IRepository
    {
        Task<List<ScheduledTask>> GetPendingTasks(DateTime currentTime);
        Task<List<ScheduledTask>> GetTaskByDevice(DixellBase device);
        Task<ScheduledTask> CreateTask(ScheduledTask task);
        Task<ScheduledTask> UpdateTask(ScheduledTask task);
        Task<ScheduledTask?> GetTaskById(Guid taskId);
        Task DeleteTask(ScheduledTask task);
        Task<List<ScheduledTask>> GetAllTasks();
        IQueryable<ScheduledTask> QueryScheduledTasks();
        Task<List<string>> GetAllTasksDeviceNamesAsync();
        /// <summary>
        /// Devuelve una lista de Tareas programadas con una cantidad de minutos definida
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        Task<List<ScheduledTask>> GetRecentTasks(int minutes);

    }
}
