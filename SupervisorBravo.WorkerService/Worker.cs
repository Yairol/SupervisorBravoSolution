using NModbus;
using NModbus.Serial;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.System;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using System.IO.Ports;

namespace SupervisorBravo.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        //Registros de los dixells
        const ushort TEMPERATURA = 256;
        const ushort SETPOINT_XR = 863;
        const ushort SETPOINT_XT = 768;
        const ushort SETPOINT_XT_OTHER = 1049;
        const ushort CONTROL_ON_OFFXR = 512;
        const ushort CONTROL_ON_OFFXT_WRITE = 1280;
        const ushort CONROL_ON_OFFXT_READ = 2049;
        const ushort THAWING_XR = 513;


        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                using (SerialPort port = new SerialPort("COM10", 9600, Parity.None, 8, StopBits.One))
                {
                    try
                    {
                        port.ReadTimeout = 3000;  // 3 segundos
                        port.WriteTimeout = 3000;
                        port.Open();


                        var factory = new ModbusFactory();
                        IModbusSerialMaster master = factory.CreateRtuMaster(port);
                        master.Transport.ReadTimeout = 3000;
                        master.Transport.WriteTimeout = 3000;

                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var repository = scope.ServiceProvider.GetRequiredService<IDixellRepository>();
                            await repository.BeginTransaction();
                            var dixells = await repository.GetAllDixells<DixellBase>();


                            //Lectura de temperatura en todos los dixells
                            foreach (var dixell in dixells)
                            {

                                try
                                {
                                    //Lectura de temperaturas.
                                    await Task.Delay(500);
                                    double? temperatureValue = await ReadTemperature(master, dixell.MoodbusId).WaitAsync(TimeSpan.FromSeconds(6));
                                    if (temperatureValue != null)
                                    {
                                        await ((ITemperatureRepository)repository).CreateTemperature(temperatureValue.Value, dixell.Id);
                                        await repository.PartialCommit();
                                    }

                                }
                                catch (TimeoutException)
                                {
                                    _logger.LogWarning($"Tiempo de espera agotado al leer temperatura del dispositivo ID {dixell.MoodbusId}");
                                    await ((ITemperatureRepository)repository).CreateTemperature(0, dixell.Id);
                                    if (await ((IDeviceAlarm)repository).GetDeviceAlarmByDeviceNamme(dixell.RoomName) == null)
                                    {
                                        await ((IDeviceAlarm)repository).CreateDeviceAlarm("Error de desconexion.", "Esta alarma se produce debido a que el dispositivo se encuentra apagado o se desconecto de el bus modbus.", dixell.RoomName);
                                    }
                                    else
                                    {
                                        var deviceAlarm = await ((IDeviceAlarm)repository).GetDeviceAlarmByDeviceNamme(dixell.RoomName);

                                    }
                                    await repository.PartialCommit();
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"Error leyendo Dixell ID {dixell.MoodbusId}");
                                }
                               await repository.PartialCommit();
                            }
                            var dixellXRs = await repository.GetAllDixells<DixellXR60CX>();
                            //Operaciones sobre Dixells --------------------------------- XR
                            foreach (var dixellx in dixellXRs)
                            {
                                var dixell = await repository.GetDixellById<DixellXR60CX>(dixellx.Id);
                                try
                                {
                                    //Escritura del control de deshielo
                                    if (dixell.ThawingWrite)
                                    {
                                        await Task.Delay(500);
                                        dixell.ThawingWrite = false;
                                        await WriteThawingXR(master, dixell.MoodbusId, dixell.Thawing).WaitAsync(TimeSpan.FromSeconds(5));

                                        await repository.UpdateDixell<DixellXR60CX>(dixell);
                                        await repository.PartialCommit();
                                    }
                                    //Escritura de control on off
                                    if (dixell.ControlON_OFFWrite)
                                    {
                                        await Task.Delay(500);
                                        await WriteControlON_OFFXR(master, dixell.MoodbusId, dixell.ControlON_OFF).WaitAsync(TimeSpan.FromSeconds(5));
                                        dixell.ControlON_OFFWrite = false;
                                        await repository.UpdateDixell<DixellXR60CX>(dixell);
                                        await repository.PartialCommit();
                                    }

                                    //Escritura de SetPoint
                                    if (dixell.SetPointWrite)
                                    {
                                        await Task.Delay(500);
                                        await WriteSetPointXR(master, dixell.MoodbusId, dixell.SetPoint).WaitAsync(TimeSpan.FromSeconds(5));
                                        dixell.SetPointWrite = false;
                                        await repository.UpdateDixell<DixellXR60CX>(dixell);
                                        await repository.PartialCommit();

                                    }

                                    //Lectura del SetPoint
                                    await Task.Delay(500);
                                    double? setPointValueXr = await ReadSetPointXR(master, dixell.MoodbusId).WaitAsync(TimeSpan.FromSeconds(5));
                                    if (setPointValueXr != null)
                                    {
                                        dixell.SetPoint = setPointValueXr.Value;
                                        await repository.UpdateDixell<DixellXR60CX>(dixell);
                                        await repository.PartialCommit();
                                    }


                                    //Lectura On/Off
                                    await Task.Delay(500);
                                    bool? controlOn_Off = await ReadControlOnOffXR(master, dixell.MoodbusId).WaitAsync(TimeSpan.FromSeconds(5));
                                    if (controlOn_Off != null)
                                    {
                                        dixell.ControlON_OFF = controlOn_Off.Value;
                                        await repository.UpdateDixell<DixellXR60CX>(dixell);
                                        await repository.PartialCommit();
                                    }


                                    //Lectura Thawing
                                    await Task.Delay(500);
                                    bool? thawing = await ReadThawingXR(master, dixell.MoodbusId).WaitAsync(TimeSpan.FromSeconds(5));
                                    if (thawing != null)
                                    {
                                        dixell.Thawing = thawing.Value;
                                        await repository.UpdateDixell<DixellXR60CX>(dixell);
                                        await repository.PartialCommit();
                                    }
                                }
                                catch (TimeoutException)
                                {
                                    _logger.LogWarning($"Tiempo de espera agotado al leer SetPoint del dispositivo ID {dixell.MoodbusId}");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"Error leyendo Dixell ID {dixell.MoodbusId}");
                                }
                            }

                            //Lectura de SetPoint y control en dixells ------------------------------------- XT
                            var dixellXTs = await repository.GetAllDixells<DixellXT111C>();
                            foreach (var dixellx in dixellXTs)
                            {
                                var dixell = await repository.GetDixellById<DixellXT111C>(dixellx.Id);
                                try
                                {
                                    
                                    //Escritura de control on off
                                    if (dixell.ControlON_OFFWrite)
                                    {
                                        await Task.Delay(500);
                                        dixell.ControlON_OFFWrite = false;
                                        await repository.UpdateDixell<DixellXT111C>(dixell);
                                        await repository.PartialCommit();
                                        await WriteControlON_OFFXT(master, dixell.MoodbusId, dixell.ControlON_OFF).WaitAsync(TimeSpan.FromSeconds(5));

                                    }
                                    //Escritura SetPoint.
                                    if (dixell.SetPointWrite)
                                    {
                                        await Task.Delay(500);
                                        dixell.SetPointWrite = false;
                                        await repository.UpdateDixell<DixellXT111C>(dixell);
                                        await repository.PartialCommit();
                                        await WriteSetPointXT(master, dixell.MoodbusId, SETPOINT_XT);
                                    }



                                    //Lectura del SetPoint
                                    await Task.Delay(500);
                                    double? setPointValueXr = await ReadSetPointXT(master, dixell.MoodbusId).WaitAsync(TimeSpan.FromSeconds(5));
                                    if (setPointValueXr != null)
                                    {
                                        dixell.SetPoint = setPointValueXr.Value;
                                        await repository.UpdateDixell<DixellXT111C>(dixell);
                                        await repository.PartialCommit();
                                    }



                                    //Lectura del Control On/Off.
                                    await Task.Delay(500);
                                    bool? controlOn_Off = await ReadControlOnOffXT(master, dixell.MoodbusId).WaitAsync(TimeSpan.FromSeconds(5));
                                    if (controlOn_Off != null)
                                    {
                                        dixell.ControlON_OFF = controlOn_Off.Value;
                                        await repository.UpdateDixell<DixellXT111C>(dixell);
                                        await repository.PartialCommit();
                                    }






                                }
                                catch (TimeoutException)
                                {
                                    _logger.LogWarning($"Tiempo de espera agotado al leer SetPoint del dispositivo ID {dixell.MoodbusId}");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"Error leyendo Dixell ID {dixell.MoodbusId}");
                                }

                            }
                            await repository.CommitTransaction();

                        }
                    }
                    catch (Exception ex)
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var repository = scope.ServiceProvider.GetRequiredService<IDixellRepository>();
                            await repository.BeginTransaction();
                            await ((IAlarmRepository)repository).CreateAlarm("Error de desconexion del bus modbus", "Este error se produce debedio a la desconexion del USB-RS485 del servidor para reconectar inserte el adaptador USB-RS485 al servidor.");
                            await repository.CommitTransaction();
                        }
                        _logger.LogError(ex, "Erro al leer el puerto");
                    }

                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            }


        }
        #region Helpers
        /// <summary>
        /// Lee la temperatura de un dixell en el bus modbus.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <returns>El valor de la temperatura.</returns>
        public async Task<double> ReadTemperature(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var temperatureUshort = await master.ReadHoldingRegistersAsync(modbusidbyte, TEMPERATURA, 1);
            double temperatureValue = ((short)temperatureUshort[0]) / 10.0;
            return temperatureValue;
        }
        /// <summary>
        /// Lee el SetPoint de un dixell XR en el bus modbus.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <returns>El valor del SetPoint.</returns>
        public async Task<double> ReadSetPointXR(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var setPointUshort = await master.ReadHoldingRegistersAsync(modbusidbyte, SETPOINT_XR, 1);
            double setPointValue = ((short)setPointUshort[0]) / 10.0;
            return setPointValue;
        }
        /// <summary>
        /// Lee el SetPoint de un dixell XT en el bus modbus.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <returns>El valor del SetPoint.</returns>
        public async Task<double> ReadSetPointXT(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var setPointUshort = await master.ReadHoldingRegistersAsync(modbusidbyte, SETPOINT_XT, 1);
            double setPointValue = (setPointUshort[0] / 10.0);
            return setPointValue;
        }
        /// <summary>
        /// Lee el estado del control on/off de un dixell XR en el bus modbus.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <returns>El estado del control on/off.</returns>
        public async Task<bool> ReadControlOnOffXR(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var controlOn_off = await master.ReadCoilsAsync(modbusidbyte, CONTROL_ON_OFFXR, 1);

            return controlOn_off[0];
        }
        /// <summary>
        /// Lee el estado del control on/off de un dixell XT en el bus modbus.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <returns>El estado del control on/off.</returns>
        public async Task<bool> ReadControlOnOffXT(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var controlOn_off = await master.ReadHoldingRegistersAsync(modbusidbyte, CONROL_ON_OFFXT_READ, 1);

            if (controlOn_off[0] == 257)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Lee el estado del deshielo en un dixell XR en el bus modbus.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificaro en el bus modbus del dixell.</param>
        /// <returns>El estado del deshielo.</returns>
        public async Task<bool> ReadThawingXR(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var thawing = await master.ReadCoilsAsync(modbusidbyte, THAWING_XR, 1);
            return thawing[0];
        }
        /// <summary>
        /// Escribe en el dixell XR el estado del deshielo.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <param name="value">Nuevo estado del dixell.</param>
        /// <returns></returns>
        public async Task WriteThawingXR(IModbusSerialMaster master, int modbusId, bool value)
        {
            var modbusidbyte = (byte)modbusId;
            if (value)
            {
                await master.WriteSingleCoilAsync(modbusidbyte, THAWING_XR, value);
            }
            else
            {
                await master.WriteSingleCoilAsync(modbusidbyte, CONTROL_ON_OFFXR, false);
                await Task.Delay(600);
                await master.WriteSingleCoilAsync(modbusidbyte, CONTROL_ON_OFFXR, true);
            }

        }
        /// <summary>
        /// Escribe en el dixell XR el estado del control on/off.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <param name="value">Nuevo estado del dixell.</param>
        /// <returns></returns>
        public async Task WriteControlON_OFFXR(IModbusSerialMaster master, int modbusId, bool value)
        {
            var modbusidbyte = (byte)modbusId;
            await master.WriteSingleCoilAsync(modbusidbyte, CONTROL_ON_OFFXR, value);
        }
        /// <summary>
        /// Escribe en el dixell XT el estado del control on/off.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <param name="value">Nuevo estado del dixell.</param>
        /// <returns></returns>
        public async Task WriteControlON_OFFXT(IModbusSerialMaster master, int modbusId, bool value)
        {
            var modbusidbyte = (byte)modbusId;
            if (value)
            {
                try
                {
                    await master.WriteSingleRegisterAsync(modbusidbyte, CONTROL_ON_OFFXT_WRITE, 65535);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "Respuesta valida 1");
                }

            }
            else
            {
                try
                {
                    await master.WriteSingleRegisterAsync(modbusidbyte, CONTROL_ON_OFFXT_WRITE, 1);
                }
                catch (IOException ex)
                {
                    _logger.LogError(ex, "Respuesta valida 1");
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
        public async Task WriteSetPointXR(IModbusMaster master, int modbusId, double value)
        {
            var modbusidbyte = (byte)modbusId;
            short valueS = ((short)(value * 10));
            ushort valueU = ((ushort)(valueS));
            await master.WriteSingleRegisterAsync(modbusidbyte, SETPOINT_XR, valueU);

        }
        /// <summary>
        /// Escribe en el dixell XT el valor del SetPoint.
        /// </summary>
        /// <param name="master">Configuracion del maestro.</param>
        /// <param name="modbusId">Identificador del dixell en el bus modbus.</param>
        /// <param name="value">Nuevo valor del SetPoint.</param>
        /// <returns></returns>
        public async Task WriteSetPointXT(IModbusMaster master, int modbusId, double value)
        {
            var modbusidbyte = (byte)modbusId;
            short valueS = ((short)(value * 10));
            ushort valueU = ((ushort)(valueS));
            try
            {
                await master.WriteSingleRegisterAsync(modbusidbyte, SETPOINT_XT, valueU);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "Respuesta valida 1");
            }
        }
        #endregion
    }
}

