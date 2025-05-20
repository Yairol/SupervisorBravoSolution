using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SupervisorBravo.Domain.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
