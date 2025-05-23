using SupervisorBravo.Domain.Entities.System;

namespace SupervisorBravo.Persistence.Abstracts.System
{
    public interface IDeviceAlarm : IRepository
    {
        Task<DeviceAlarm> CreateDeviceAlarm(string name, string description, string deviceName);
        Task<DeviceAlarm> GetDeviceAlarmById(Guid id);
        Task<DeviceAlarm?> GetDeviceAlarmByDeviceNamme(string deviceName);
        Task UpdateDate(DeviceAlarm deviceAlarm);
        Task<IEnumerable<DeviceAlarm>> GetAllDeviceAlarms();
        Task DeleteDeviceAlarm(Guid deviceAlarmId);
    }
}
