using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Schedule;
using SupervisorBravo.Domain.Entities.System;
using SupervisorBravo.Persistence.Abstracts.ScheduledTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IScheduledTaskRepository
    {

        public async Task<List<ScheduledTask>> GetPendingTasks(DateTime currentTime)
        {
            return await _context.Set<ScheduledTask>()
                .Where(t => t.Status == ScheduledTaskStatus.Pending && t.ScheduledDateTime <= currentTime)
                .ToListAsync();
        }

        public async Task<List<ScheduledTask>> GetTaskByDevice(DixellBase device)
        {
            return await _context.Set<ScheduledTask>()
                .Where(t => t.DeviceId == device.Id)
                .ToListAsync();
        }

        public async Task<ScheduledTask> CreateTask(ScheduledTask task)
        {
            if (_context == null)
                throw new InvalidOperationException("No hay contexto activo. Usa BeginTransaction.");

            task.Id = Guid.NewGuid(); // si no se asigna automáticamente
            task.ScheduledDateTime = task.ScheduledDateTime.ToUniversalTime();

            if (task.RecurrenceEndDate.HasValue)
                task.RecurrenceEndDate = task.RecurrenceEndDate.Value.ToUniversalTime();

            await _context.Set<ScheduledTask>().AddAsync(task);
            return task;
        }



        public async Task<ScheduledTask> UpdateTask(ScheduledTask task)
        {
            _context.Set<ScheduledTask>()
                .Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<ScheduledTask?> GetTaskById(Guid taskId)
        {
            return await _context
                .Set<ScheduledTask>()
                .FindAsync(taskId);
        }
        public async Task DeleteTask(ScheduledTask task)
        {
            var TaskToDelete = await _context
                .Set<ScheduledTask>()
                .FindAsync(task.Id);

            if (task != null)
            {
                _context.Remove(task);
            }
        }
        public async Task<List<ScheduledTask>> GetAllTasks()
        {
            return await _context.ScheduledTasks
                .Include(t => t.Device)
                .ToListAsync();
        }

    }

}
