using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.System;
using SupervisorBravo.Persistence.Abstracts.System;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IAlarmRepository
    {
        public async Task<Alarm> CreateAlarm(string name, string description)
        {
            var ctx = EnsureContext();
            var alarm = new Alarm(name, description);
            await ctx.AddAsync(alarm);
            return alarm;
        }

        public async Task DeleteAlarm(Guid alarmId)
        {
            var ctx = EnsureContext();
            var alarm = await ctx.Set<Alarm>().FindAsync(alarmId);
            if (alarm is not null)
            {
                ctx.Remove(alarm);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<Alarm> GetAlarmById(Guid id)
        {
            var ctx = EnsureContext();
            return await ctx.Set<Alarm>().FindAsync(id)
                   ?? throw new KeyNotFoundException($"Alarm con Id {id} no encontrada");
        }


        public async Task<IEnumerable<Alarm>> GetAllAlarms()
        {
            var ctx = EnsureContext();
            return await ctx.Set<Alarm>().ToListAsync();
        }
    }
}
