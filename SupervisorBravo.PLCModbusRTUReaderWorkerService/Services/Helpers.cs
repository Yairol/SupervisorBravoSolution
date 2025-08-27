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
        /// <summary>
        /// para los numeros muy grandes, tener en cuenta que el primero que se lee es el LowRegister
        /// Osea si:
        /// R0= 5540 y R1 = 2
        /// HighRegister R1, LowRegister R0
        /// </summary>
        /// <param name="HighRegister"></param>
        /// <param name="LowRegister"></param>
        /// <returns></returns>
        public static double ConvertModbusToInt(ushort HighRegister, ushort LowRegister)
        {
            double valor = (double)(HighRegister * ushort.MaxValue);
            valor += (double)LowRegister;
            return valor;
        }

    }
}
