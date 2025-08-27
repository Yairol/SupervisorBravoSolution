using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Schedule;
using SupervisorBravo.Persistence.Abstracts.Dixells;

namespace SupervisorBravo.ScheduledTaskWorkerService.Helpers
{
    public class ScheduledTaskHelpers
    {
        public static async Task<bool> CheckTasksExecution(IDixellRepository _dixellRepository, ScheduledTask task)
        {
            bool TransactionFlag = true;
            if (_dixellRepository == null) return false;
            if (task == null) return false;
            if (!_dixellRepository.IsInTransaction)
            {
                await _dixellRepository.BeginTransaction();
                TransactionFlag = false;
            }
            var device = await _dixellRepository.GetDixellById<DixellXR>(task.DeviceId);
            if (TransactionFlag)
                await _dixellRepository.PartialCommit();
            else await _dixellRepository.CommitTransaction();

            if (device == null) return false;
            switch (task.Action)
            {
                case ActionType.TurnOn:
                    if (!device.ControlON_OFF && !device.ControlON_OFFWrite)
                    {
                        return false;
                    }
                    else return true;
                case ActionType.TurnOff:
                    if (device.ControlON_OFF && !device.ControlON_OFFWrite)
                    {
                        return false;
                    }
                    else return true;
                case ActionType.ChangeSetPoint:
                    if (device.SetPoint != task.SetPointValue && !device.SetPointWrite)
                    {
                        return false;
                    }
                    else return true;
                default: return false;
            }

        }

    }
}
