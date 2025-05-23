using Microsoft.AspNetCore.Mvc;
using SupervisorBravo.Persistence.Abstracts.System;
using SupervisorBravo.Web.Models.DTOs;

namespace SupervisorBravo.Web.Controllers.System.Alarms
{
    public class AlarmController : Controller
    {
        private readonly IAlarmRepository _alarmRepository;

        public AlarmController(IAlarmRepository alarmRepository)
        {
            _alarmRepository = alarmRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Alarms()
        {
            await _alarmRepository.BeginTransaction();

            var alarms = await _alarmRepository.GetAllAlarms();
            var deviceAlarms = await ((IDeviceAlarm)_alarmRepository).GetAllDeviceAlarms();

            var alarmsList = alarms.OrderByDescending(x => x.AlarmDate).ToList();
            var deviceAlarmsList = deviceAlarms.OrderByDescending(x => x.AlarmDate).ToList();

            var alarmModel = new AlarmModel(alarmsList, deviceAlarmsList);

            await _alarmRepository.CommitTransaction();
            return View(alarmModel);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteAlarm(Guid id)
        {
            await _alarmRepository.BeginTransaction();
            var alarm = await _alarmRepository.GetAlarmById(id);
            if (alarm != null)
            {
                await _alarmRepository.DeleteAlarm(id);
            }
            await _alarmRepository.CommitTransaction();
            return RedirectToAction(nameof(Alarms));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteAllAlarms()
        {
            await _alarmRepository.BeginTransaction();
            var alarms = await _alarmRepository.GetAllAlarms();
            foreach (var alarm in alarms)
            {
                await _alarmRepository.DeleteAlarm(alarm.Id);
            }
            await _alarmRepository.CommitTransaction();
            return RedirectToAction(nameof(Alarms));
        }
        [HttpGet]
        public async Task<IActionResult> DeleteAllDeviceAlarms()
        {
            await _alarmRepository.BeginTransaction();
            var alarms = await ((IDeviceAlarm)_alarmRepository).GetAllDeviceAlarms();
            foreach (var alarm in alarms)
            {
                await ((IDeviceAlarm)_alarmRepository).DeleteDeviceAlarm(alarm.Id);
            }
            await _alarmRepository.CommitTransaction();
            return RedirectToAction(nameof(Alarms));
        }
    }
}
