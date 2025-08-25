using NModbus;
using SupervisorBravo.WorkerService.Utilities;

namespace SupervisorBravo.WorkerService
{
    public static class ModbusWorkerServiceHelpers
    {
        #region Helpers
        /// <summary>
        /// Lee la temperatura de un dixell en el bus modbus.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <returns>El valor de la temperatura.</returns>
        public static async Task<double> ReadTemperature(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var temperatureUshort = await master.ReadHoldingRegistersAsync(modbusidbyte, 256, 1);
            double temperatureValue = ((short)temperatureUshort[0]) / 10.0;
            return temperatureValue;
        }
        /// <summary>
        /// Lee el SetPoint de un dixell XR en el bus modbus.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <returns>El valor del SetPoint.</returns>
        public static async Task<double> ReadSetPointXR(IModbusSerialMaster master, int modbusId, string modelName)
        {
            var modbusidbyte = (byte)modbusId;
            var setPointUshort = await master.ReadHoldingRegistersAsync(modbusidbyte, GetRegistersbyModel.GetXRSetPointRegisterByModel(modelName), 1);
            Console.WriteLine($"se leyo la direccion {GetRegistersbyModel.GetXRSetPointRegisterByModel(modelName)}");
            //Esto es para leer los XR160 porque el set point es otro registro
            /* if (setPointUshort[0] == (ushort)144)
             {
                 setPointUshort = await master.ReadHoldingRegistersAsync(modbusidbyte, 876, 1);
             }*/
            double setPointValue = ((short)setPointUshort[0]) / 10.0;
            return setPointValue;
        }
        /// <summary>
        /// Lee el SetPoint de un dixell XT en el bus modbus.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <returns>El valor del SetPoint.</returns>
        public static async Task<double> ReadSetPointXT(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var setPointUshort = await master.ReadHoldingRegistersAsync(modbusidbyte, 768, 1);
            double setPointValue = (setPointUshort[0] / 10.0);
            return setPointValue;
        }
        /// <summary>
        /// Lee el estado del control on/off de un dixell XR en el bus modbus.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <returns>El estado del control on/off.</returns>
        public static async Task<bool> ReadControlOnOffXR(IModbusSerialMaster master, int modbusId, string modelName)
        {
            var modbusidbyte = (byte)modbusId;
            if (GetRegistersbyModel.GetRegisterTypeByModel(modelName) == false)
            {
                var controlOn_off = await master.ReadCoilsAsync(modbusidbyte, 512, 1);
                return controlOn_off[0];
            }
            else
            {
                var controlOn_off = await master.ReadHoldingRegistersAsync(modbusidbyte, GetRegistersbyModel.GetXROnOffRegisterByModel(modelName), 1);
                bool result = GetRegistersbyModel.GetXROnOffResultByModel(modelName, controlOn_off[0]);
                return result;

            }

        }
        /// <summary>
        /// Lee el estado del control on/off de un dixell XT en el bus modbus.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <returns>El estado del control on/off.</returns>
        public static async Task<bool> ReadControlOnOffXT(IModbusSerialMaster master, int modbusId)
        {
            const ushort MASK_ON = 1 << 8;  // bit 8 indica ENCENDIDO/APAGADO

            byte id = (byte)modbusId;
            ushort[] regs;

            try
            {
                // leer un solo registro
                regs = await master.ReadHoldingRegistersAsync(id, 1280, 1);
            }
            catch (TimeoutException)
            {
                // no responde ? consideramos APAGADO
                return false;
            }
            catch
            {
                // otros errores de comunicación ? APAGADO por defecto
                return false;
            }

            ushort value = regs[0];
            // bit 8 = 1 ? ENCENDIDO; = 0 ? APAGADO
            return (value & MASK_ON) != 0;
        }

        /// <summary>
        /// Lee el estado del deshielo en un dixell XR en el bus modbus.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificaro en el bus modbus del dixell.</param>
        /// <returns>El estado del deshielo.</returns>
        public static async Task<bool> ReadThawingXR(IModbusSerialMaster master, int modbusId, string modelName)
        {
            var modbusidbyte = (byte)modbusId;
            if (GetRegistersbyModel.GetRegisterTypeByModel(modelName) == false)
            {
                var thawing = await master.ReadCoilsAsync(modbusidbyte, GetRegistersbyModel.GetXRThawingRegisterByModel(modelName), 1);
                return thawing[0];
            }
            else
            {
                var thawing = await master.ReadHoldingRegistersAsync(modbusidbyte, GetRegistersbyModel.GetXRThawingRegisterByModel(modelName), 1);
                bool result = GetRegistersbyModel.GetXRThawingResultByModel(modelName, thawing[0]);
                return result;

            }

        }
        /// <summary>
        /// Escribe en el dixell XR el estado del deshielo.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <param name="value">Nuevo estado del dixell.</param>
        /// <returns></returns>
        public static async Task WriteThawingXR(IModbusSerialMaster master, int modbusId, bool value, string modelName)
        {
            var modbusidbyte = (byte)modbusId;
            if (value)
            {
                if (GetRegistersbyModel.GetRegisterTypeByModel(modelName))
                {
                    await master.WriteSingleRegisterAsync(modbusidbyte, GetRegistersbyModel.GetXRThawingRegisterByModel(modelName), GetRegistersbyModel.GetXRThawingWriteResultByModel(modelName, value));
                }
                else
                {
                    await master.WriteSingleCoilAsync(modbusidbyte, GetRegistersbyModel.GetXRThawingRegisterByModel(modelName), value);
                }

            }
            else
            {
                if (GetRegistersbyModel.GetRegisterTypeByModel(modelName))
                {
                    await master.WriteSingleRegisterAsync(modbusidbyte, GetRegistersbyModel.GetXROnOffWriteRegisterByModel(modelName), GetRegistersbyModel.GetXROnOffWriteResultByModel(modelName, false));
                    await Task.Delay(600);
                    await master.WriteSingleRegisterAsync(modbusidbyte, GetRegistersbyModel.GetXROnOffWriteRegisterByModel(modelName), GetRegistersbyModel.GetXROnOffWriteResultByModel(modelName, true));

                    //await master.WriteSingleRegisterAsync(modbusidbyte, GetRegistersbyModel.GetXRThawingRegisterByModel(modelName), GetRegistersbyModel.GetXRThawingWriteResultByModel(modelName, false));

                }
                else
                {
                    await master.WriteSingleCoilAsync(modbusidbyte, 512, false);
                    await Task.Delay(600);
                    await master.WriteSingleCoilAsync(modbusidbyte, 512, true);
                }

            }

        }
        /// <summary>
        /// Escribe en el dixell XR el estado del control on/off.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <param name="value">Nuevo estado del dixell.</param>
        /// <returns></returns>
        public static async Task WriteControlON_OFFXR(IModbusSerialMaster master, int modbusId, bool value, string modelName)
        {
            var modbusidbyte = (byte)modbusId;
            if (GetRegistersbyModel.GetRegisterTypeByModel(modelName) == false)
            {
                await master.WriteSingleCoilAsync(modbusidbyte, GetRegistersbyModel.GetXROnOffWriteRegisterByModel(modelName), value);
            }
            else
            {
                await master.WriteSingleRegisterAsync(modbusidbyte, GetRegistersbyModel.GetXROnOffWriteRegisterByModel(modelName), GetRegistersbyModel.GetXROnOffWriteResultByModel(modelName, value));
            }

        }
        /// <summary>
        /// Escribe en el dixell XT el estado del control on/off.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <param name="value">Nuevo estado del dixell.</param>
        /// <returns></returns>
        public static async Task WriteControlON_OFFXT(IModbusSerialMaster master, int modbusId, bool value)
        {
            var modbusidbyte = (byte)modbusId;
            if (value)
            {
                try
                {
                    await master.WriteSingleRegisterAsync(modbusidbyte, 1280, 65535);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex + "Respuesta valida 1");
                }

            }
            else
            {
                try
                {
                    await master.WriteSingleRegisterAsync(modbusidbyte, 1280, 1);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex + " Respuesta valida 1");
                }
            }
        }
        /// <summary>
        /// Escribe en el dixell XR el valor del SetPoint.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <param name="value">Nuevo valor del SetPoint.</param>
        /// <returns></returns>
        public static async Task WriteSetPointXR(IModbusMaster master, int modbusId, double value, string modelName)
        {
            var modbusidbyte = (byte)modbusId;
            short valueS = ((short)(value * 10));
            ushort valueU = ((ushort)(valueS));
            await master.WriteSingleRegisterAsync(modbusidbyte, GetRegistersbyModel.GetXRSetPointRegisterByModel(modelName), valueU);

        }
        /// <summary>
        /// Escribe en el dixell XT el valor del SetPoint.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <param name="value">Nuevo valor del SetPoint.</param>
        /// <returns></returns>
        public static async Task WriteSetPointXT(IModbusMaster master, int modbusId, double value)
        {
            var modbusidbyte = (byte)modbusId;
            short valueS = ((short)(value * 10));
            //int valueI = ((int)(value * 10));
            ushort valueU = ((ushort)(valueS));
            await master.WriteSingleRegisterAsync(modbusidbyte, 1049, valueU);
        }
        #endregion
    }
}
