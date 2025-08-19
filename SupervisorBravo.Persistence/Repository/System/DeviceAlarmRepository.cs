using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.System;
using SupervisorBravo.Persistence.Abstracts.System;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IDeviceAlarm
    {
        public async Task<DeviceAlarm> CreateDeviceAlarm(string name, string description, string deviceName)
        {
            var ctx = EnsureContext();
            DeviceAlarm deviceAlarm = new DeviceAlarm(name, description, deviceName);
            await ctx.Set<DeviceAlarm>().AddAsync(deviceAlarm);
            return deviceAlarm;
        }

        public async Task DeleteAlarmByRoomName(string roomName)
        {
            var ctx = EnsureContext();
            var deviceAlarm = await ctx.Set<DeviceAlarm>().FirstOrDefaultAsync(d => d.DeviceName == roomName);
            if (deviceAlarm is not null)
            {
                ctx.Remove(deviceAlarm);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task DeleteDeviceAlarm(Guid deviceAlarmId)
        {
            var ctx = EnsureContext();
            var deviceAlarm = await ctx.Set<DeviceAlarm>().FindAsync(deviceAlarmId);
            if (deviceAlarm is not null)
            {
                ctx.Remove(deviceAlarm);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<DeviceAlarm>> GetAllDeviceAlarms()
        {
            var ctx = EnsureContext();
            return await ctx.Set<DeviceAlarm>().ToListAsync();
        }

        public async Task<DeviceAlarm?> GetDeviceAlarmByDeviceNamme(string deviceName)
        {
            var ctx = EnsureContext();
            return await ctx.Set<DeviceAlarm>().FirstOrDefaultAsync(d => d.DeviceName == deviceName);
        }

        public async Task<DeviceAlarm> GetDeviceAlarmById(Guid id)
        {
            var ctx = EnsureContext();
            return await ctx.Set<DeviceAlarm>().FindAsync(id)
                   ?? throw new KeyNotFoundException($"DeviceAlarm con Id {id} no encontrado");
        }


        public Task UpdateDate(DeviceAlarm deviceAlarm)
        {
            var ctx = EnsureContext();
            ctx.Set<DeviceAlarm>().Update(deviceAlarm);
            return Task.CompletedTask;
        }
    }
}
