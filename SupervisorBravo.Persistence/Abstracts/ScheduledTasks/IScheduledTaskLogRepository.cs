using SupervisorBravo.Domain.Entities.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence.Abstracts.ScheduledTasks
{
    public interface IScheduledTaskExecutionLogRepository
    {
        Task<List<ScheduledTaskExecutionLog>> GetLogByTask(ScheduledTask task);
        Task<ScheduledTaskExecutionLog> AddLog(ScheduledTaskExecutionLog log);
        Task<List<ScheduledTaskExecutionLog>> GetRecentLogs(int maxCount = 20);
    }
}
