using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.PLCModbusRTUReaderWorkerService.Services
{
    public class Helpers
    {
        public Helpers() { }
        public static double ConvertModbusToFloat(ushort highRegister, ushort lowRegister)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(lowRegister & 0xFF);
            bytes[1] = (byte)((lowRegister >> 8) & 0xFF);
            bytes[2] = (byte)(highRegister & 0xFF);
            bytes[3] = (byte)((highRegister >> 8) & 0xFF);

            return BitConverter.ToSingle(bytes, 0);
        }
        public static bool GetBitValue(ushort value, int bitIndex)
        {
            if (bitIndex < 0 || bitIndex > 15)
                throw new ArgumentOutOfRangeException(nameof(bitIndex), "El índice debe estar entre 0 y 15.");

            return ((value >> bitIndex) & 1) == 1;
        }

    }
}
