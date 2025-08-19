
using NModbus;
using NModbus.Serial;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.PLCModbusRTUReaderWorkerService.Services
{
    public class ModbusReaderService : IModbusReaderService
    {
        private readonly string _portName; // Ej: "COM3"
        private readonly int _baudRate;
        private readonly Parity _parity;
        private readonly int _dataBits;
        private readonly StopBits _stopBits;

        public ModbusReaderService(IConfiguration config)
        {
            _portName = config["ModbusRTU:PortName"]
                ?? throw new ArgumentNullException("ModbusRTU:PortName no configurado");

            var baudRateString = config["ModbusRTU:BaudRate"]
                ?? throw new ArgumentNullException("ModbusRTU:BaudRate no configurado");
            _baudRate = int.Parse(baudRateString);

            var parityString = config["ModbusRTU:Parity"]
                ?? throw new ArgumentNullException("ModbusRTU:Parity no configurado");
            _parity = Enum.Parse<Parity>(parityString);

            var dataBitsString = config["ModbusRTU:DataBits"]
                ?? throw new ArgumentNullException("ModbusRTU:DataBits no configurado");
            _dataBits = int.Parse(dataBitsString);

            var stopBitsString = config["ModbusRTU:StopBits"]
                ?? throw new ArgumentNullException("ModbusRTU:StopBits no configurado");
            _stopBits = Enum.Parse<StopBits>(stopBitsString);
        }


        public async Task<bool> ReadCoilAsync(byte unitId, ushort address)
        {
            using var port = new SerialPort(_portName, _baudRate, _parity, _dataBits, _stopBits);
            port.Open();
            var factory = new ModbusFactory();
            var master = factory.CreateRtuMaster(port);
            var result = await master.ReadCoilsAsync(unitId, address, 1);
            port.Close();
            return result[0];
        }

        public async Task<ushort[]> ReadHoldingRegisterAsync(byte unitId, ushort address)
        {
            using var port = new SerialPort(_portName, _baudRate, _parity, _dataBits, _stopBits);
            port.Open();
            var address2 = (ushort)(address + 1);
            var factory = new ModbusFactory();
            var master = factory.CreateRtuMaster(port);
            var Reg1 = await master.ReadHoldingRegistersAsync(unitId, address, 2);
            var Reg2 = await master.ReadHoldingRegistersAsync(unitId, address2, 2);
            ushort[] result = { Reg1[0], Reg2[0] };
            port.Close();
            return result;
        }
    }

}
