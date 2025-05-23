using SupervisorBravo.Domain.Entities.System;

namespace SupervisorBravo.Persistence.Abstracts.System
{
    public interface IAlarmRepository : IRepository
    {
        Task<Alarm> CreateAlarm(string name, string description);
        Task<Alarm> GetAlarmById(Guid id);
        Task<IEnumerable<Alarm>> GetAllAlarms();
        Task DeleteAlarm(Guid alarmId);
    }
}
