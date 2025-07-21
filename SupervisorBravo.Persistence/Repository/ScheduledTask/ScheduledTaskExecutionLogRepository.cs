using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Schedule;
using SupervisorBravo.Persistence.Abstracts.ScheduledTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IScheduledTaskExecutionLogRepository
    {

        public async Task<List<ScheduledTaskExecutionLog>> GetLogByTask(ScheduledTask task)
        {
            return await _context.Set<ScheduledTaskExecutionLog>()
                .Where(log => log.TaskId == task.Id)
                .OrderByDescending(log => log.Timestamp)
                .ToListAsync();
        }

        public async Task<List<ScheduledTaskExecutionLog>> GetRecentLogs(int maxCount = 25)
        {
            return await _context.Set<ScheduledTaskExecutionLog>()
                .OrderByDescending(log => log.Timestamp)
                .Take(maxCount)
                .ToListAsync();
        }

        public async Task<ScheduledTaskExecutionLog> AddLog(ScheduledTaskExecutionLog log)
        {
            await _context.Set<ScheduledTaskExecutionLog>().AddAsync(log);
            await _context.SaveChangesAsync();
            return log;
        }
    }

}
