using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.WorkerService.Utilities
{
    public interface IModbusService
    {
        Task<ushort[]> ReadRegistersAsync(byte slaveId, ushort startAddress, ushort numberOfPoints);


    }
}
