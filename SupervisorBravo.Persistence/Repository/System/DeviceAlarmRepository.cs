using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.System;
using SupervisorBravo.Persistence.Abstracts.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IDeviceAlarm
    {
        public async Task<DeviceAlarm> CreateDeviceAlarm(string name, string description, string deviceName)
        {
            DeviceAlarm deviceAlarm = new DeviceAlarm(name, description, deviceName);
            await _context.Set<DeviceAlarm>().AddAsync(deviceAlarm);
            return deviceAlarm;
        }

        public async Task DeleteDeviceAlarm(Guid deviceAlarmId)
        {
            var deviceAlarm = await _context.Set<DeviceAlarm>().FindAsync(deviceAlarmId);
            if(deviceAlarm != null)
            {
                _context.Remove(deviceAlarm);
            }

        }

        public async Task<IEnumerable<DeviceAlarm>> GetAllDeviceAlarms()
        {
            var deviceAlarms = await _context.Set<DeviceAlarm>().ToListAsync();
            return deviceAlarms;
        }

        public async Task<DeviceAlarm?> GetDeviceAlarmByDeviceNamme(string deviceName)
        {
            return await _context.Set<DeviceAlarm>().FirstOrDefaultAsync(d => d.DeviceName == deviceName);
        }

        public async Task<DeviceAlarm> GetDeviceAlarmById(Guid id)
        {
            return await _context.Set<DeviceAlarm>().FindAsync(id);
        }

        public Task UpdateDate(DeviceAlarm deviceAlarm)
        {
            _context.Set<DeviceAlarm>().Update(deviceAlarm);
            return Task.CompletedTask;
        }
    }
}
