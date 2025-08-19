using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Schedule;
using SupervisorBravo.Domain.Entities.System;
using SupervisorBravo.Persistence.Abstracts.ScheduledTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IScheduledTaskRepository
    {
        public async Task<List<ScheduledTask>> GetPendingTasks(DateTime currentTime)
        {
            var ctx = EnsureContext();
            return await ctx.Set<ScheduledTask>()
                .Where(t => t.Status == ScheduledTaskStatus.Pending && t.ScheduledDateTime <= currentTime)
                .ToListAsync();
        }

        public async Task<List<ScheduledTask>> GetTaskByDevice(DixellBase device)
        {
            var ctx = EnsureContext();
            return await ctx.Set<ScheduledTask>()
                .Where(t => t.DeviceId == device.Id)
                .ToListAsync();
        }

        public async Task<ScheduledTask> CreateTask(ScheduledTask task)
        {
            var ctx = EnsureContext();

            task.Id = Guid.NewGuid(); // si no se asigna automáticamente
            task.ScheduledDateTime = task.ScheduledDateTime.ToUniversalTime();

            if (task.RecurrenceEndDate.HasValue)
                task.RecurrenceEndDate = task.RecurrenceEndDate.Value.ToUniversalTime();

            await ctx.Set<ScheduledTask>().AddAsync(task);
            return task;
        }

        public async Task<ScheduledTask> UpdateTask(ScheduledTask task)
        {
            var ctx = EnsureContext();
            ctx.Set<ScheduledTask>().Update(task);
            await ctx.SaveChangesAsync();
            return task;
        }

        public async Task<ScheduledTask?> GetTaskById(Guid taskId)
        {
            var ctx = EnsureContext();
            return await ctx.Set<ScheduledTask>().FindAsync(taskId);
        }

        public async Task DeleteTask(ScheduledTask task)
        {
            var ctx = EnsureContext();
            var taskToDelete = await ctx.Set<ScheduledTask>().FindAsync(task.Id);

            if (taskToDelete is not null)
            {
                ctx.Remove(taskToDelete);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<List<ScheduledTask>> GetAllTasks()
        {
            var ctx = EnsureContext();
            return await ctx.ScheduledTasks
                .Include(t => t.Device)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllTasksDeviceNamesAsync()
        {
            var ctx = EnsureContext();
            return await ctx.Set<ScheduledTask>()
                .Include(t => t.Device)
                .Where(t => t.Device != null && !string.IsNullOrWhiteSpace(t.Device.RoomName))
                .Select(t => t.Device.RoomName)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync();
        }

        public IQueryable<ScheduledTask> QueryScheduledTasks()
        {
            var ctx = EnsureContext();
            return ctx.Set<ScheduledTask>().AsQueryable();
        }
    }
}
