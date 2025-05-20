using SupervisorBravo.Domain.Entities.System;

namespace SupervisorBravo.Web.Models.DTOs
{
    public class AlarmModel
    {
        public List<Alarm> Alarms { get; set; }
        public List<DeviceAlarm> DeviceAlarms { get; set; }

        public AlarmModel(List<Alarm> alarms, List<DeviceAlarm> deviceAlarms)
        {
            Alarms = alarms;
            DeviceAlarms = deviceAlarms;
        }
    }
}
