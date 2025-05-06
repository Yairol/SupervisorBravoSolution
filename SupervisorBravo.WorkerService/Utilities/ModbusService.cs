using NModbus;
using NModbus.Serial;
using System.IO.Ports;

namespace SupervisorBravo.WorkerService.Utilities
{
    public class ModbusService : IModbusService, IDisposable
    {
        private readonly SerialPort _serialPort;
        private readonly IModbusSerialMaster _master;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public ModbusService()
        {
            _serialPort = new SerialPort("COM9", 9600, Parity.None, 8, StopBits.One);
            _serialPort.Open();
            _master = new ModbusFactory().CreateRtuMaster(_serialPort);
            _master.Transport.ReadTimeout = 2000;
        }

        public async Task<ushort[]> ReadRegistersAsync(byte slaveId, ushort startAddress, ushort numberOfPoints)
        {
            await _mutex.WaitAsync();
            try
            {
                return await _master.ReadHoldingRegistersAsync(slaveId, startAddress, numberOfPoints);
            }
            catch (IOException ex)
            {
                // Timeout o desconexión física
                throw new TimeoutException("No se obtuvo respuesta del dispositivo", ex);
            }
            finally
            {
                _mutex.Release();
            }
        }

        public void Dispose()
        {
            _serialPort?.Close();
            _serialPort?.Dispose();
            _mutex?.Dispose();
        }

    }
}
