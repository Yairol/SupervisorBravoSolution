using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Schedule;

namespace SupervisorBravo.Persistence.Abstracts.ScheduledTasks
{
    public interface IScheduledTaskRepository : IRepository
    {
        Task<List<ScheduledTask>> GetPendingTasks(DateTime currentTime);
        Task<List<ScheduledTask>> GetTaskByDevice(DixellBase device);
        Task<ScheduledTask> CreateTask(ScheduledTask task);
        Task <ScheduledTask>UpdateTask(ScheduledTask task);
        Task<ScheduledTask?> GetTaskById(Guid taskId);
        Task DeleteTask(ScheduledTask task);
        Task<List<ScheduledTask>> GetAllTasks();
    }
}
