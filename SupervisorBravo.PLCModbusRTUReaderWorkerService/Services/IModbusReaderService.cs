using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.PLCModbusRTUReaderWorkerService.Services
{
    public interface IModbusReaderService
    {
        Task<bool> ReadCoilAsync( byte unitId, ushort address);
        Task<ushort[]> ReadHoldingRegisterAsync( byte unitId, ushort address);
    }

}
