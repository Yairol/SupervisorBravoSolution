using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.System;
using SupervisorBravo.Persistence.Abstracts.System;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IAlarmRepository
    {
        public async Task<Alarm> CreateAlarm(string name, string description)
        {
            var alarm = new Alarm(name, description);
            await _context.AddAsync(alarm);
            return alarm;
        }

        public async Task DeleteAlarm(Guid alarmId)
        {
            var alarm = await _context.Set<Alarm>().FindAsync(alarmId);
            if (alarm != null)
            {
                _context.Remove(alarm);
            }
        }

        public async Task<Alarm> GetAlarmById(Guid id)
        {
            return await _context.Set<Alarm>().FindAsync(id);
        }

        public async Task<IEnumerable<Alarm>> GetAllAlarms()
        {
            return await _context.Set<Alarm>().ToListAsync();
        }
    }
}
