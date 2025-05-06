using NModbus;
using NModbus.Serial;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using System.IO.Ports;

namespace SupervisorBravo.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
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
                using (SerialPort port = new SerialPort("COM9", 9600, Parity.None, 8, StopBits.One))
                {
                    try
                    {
                        port.Open();
                        port.ReadTimeout = 3000;  // 3 segundos
                        port.WriteTimeout = 3000;

                        var factory = new ModbusFactory();
                        IModbusSerialMaster master = factory.CreateRtuMaster(port);
                        master.Transport.ReadTimeout = 3000;
                        master.Transport.WriteTimeout = 3000;
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var repository = scope.ServiceProvider.GetRequiredService<IDixellRepository>();
                            await repository.BeginTransaction();
                            var dixells = await repository.GetAllDixells<DixellBase>();
                            var dixellXRs = await repository.GetAllDixells<DixellXR60CX>();
                            var dixellXTs = await repository.GetAllDixells<DixellXT111C>();

                            //Lectura de temperatura en todos los dixells
                            foreach (var dixell in dixells)
                            {
                                try
                                {
                                    //Lectura de temperaturas.
                                    double? temperatureValue = await ReadTemperature(master, dixell.MoodbusId).WaitAsync(TimeSpan.FromSeconds(5));
                                    if (temperatureValue != null)
                                        await ((ITemperatureRepository)repository).CreateTemperature(temperatureValue.Value, dixell.Id);
                                    await Task.Delay(500);

                                    await repository.PartialCommit();
                                }
                                catch (TimeoutException)
                                {
                                    _logger.LogWarning($"Tiempo de espera agotado al leer temperatura del dispositivo ID {dixell.MoodbusId}");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, $"Error leyendo Dixell ID {dixell.MoodbusId}");
                                }
                            }
                            //Lectura de SetPoint en dixells XR
                            foreach (var dixell in dixellXRs)
                            {
                                try
                                {
                                    //Lectura del SetPoint
                                    await Task.Delay(500);
                                    double? setPointValueXr = await ReadSetPointXR(master, dixell.MoodbusId).WaitAsync(TimeSpan.FromSeconds(5));
                                    if (setPointValueXr != null)
                                    {
                                        dixell.SetPoint = setPointValueXr.Value;
                                        await repository.UpdateDixell<DixellXR60CX>(dixell);
                                        await repository.PartialCommit();
                                    }
                                    await Task.Delay(500);

                                    //Lectura On/Off
                                    bool? controlOn_Off = await ReadControlOnOffXR(master, dixell.MoodbusId).WaitAsync(TimeSpan.FromSeconds(5));
                                    if (controlOn_Off != null)
                                    {
                                        dixell.ControlON_OFF = controlOn_Off.Value;
                                        await repository.UpdateDixell<DixellXR60CX>(dixell);
                                        await repository.PartialCommit();
                                    }
                                    await Task.Delay(500);

                                    //Lectura Thawing
                                    bool? thawing = await ReadThawingXR(master, dixell.MoodbusId).WaitAsync(TimeSpan.FromSeconds(5));
                                    if (thawing != null)
                                    {
                                        dixell.Thawing = thawing.Value;
                                        await repository.UpdateDixell<DixellXR60CX>(dixell);
                                        await repository.PartialCommit();
                                    }
                                    await Task.Delay(500);


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
                            await Task.Delay(500);
                            //Lectura de SetPoint y control en dixells XT
                            foreach (var dixell in dixellXTs)
                            {
                                try
                                {
                                    await Task.Delay(500);
                                    //Lectura del SetPoint
                                    double? setPointValueXr = await ReadSetPointXT(master, dixell.MoodbusId).WaitAsync(TimeSpan.FromSeconds(5));
                                    if (setPointValueXr != null)
                                    {
                                        dixell.SetPoint = setPointValueXr.Value;
                                        await repository.UpdateDixell<DixellXT111C>(dixell);
                                        await repository.PartialCommit();
                                    }
                                    await Task.Delay(500);


                                    //Lectura del Control On/Off.
                                    bool? controlOn_Off = await ReadControlOnOffXT(master, dixell.MoodbusId).WaitAsync(TimeSpan.FromSeconds(5));
                                    if (controlOn_Off != null)
                                    {
                                        dixell.ControlON_OFF = controlOn_Off.Value;
                                        await repository.UpdateDixell<DixellXT111C>(dixell);
                                    }
                                    await repository.PartialCommit();
                                    await Task.Delay(500);

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
                        _logger.LogError(ex, "Erro al leer el puerto");
                    }

                }
                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);

            }


        }

        public async Task<double> ReadTemperature(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var temperatureUshort = await master.ReadHoldingRegistersAsync(modbusidbyte, 256, 1);
            double temperatureValue = ((short)temperatureUshort[0]) / 10.0;
            return temperatureValue;
        }

        public async Task<double> ReadSetPointXR(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var setPointUshort = await master.ReadHoldingRegistersAsync(modbusidbyte, SETPOINT_XR, 1);
            double setPointValue = ((short)setPointUshort[0]) / 10.0 ;
            return setPointValue;
        }

        public async Task<double> ReadSetPointXT(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var setPointUshort = await master.ReadHoldingRegistersAsync(modbusidbyte, SETPOINT_XT, 1);
            double setPointValue = (setPointUshort[0] / 10.0);
            return setPointValue;
        }

        public async Task<bool> ReadControlOnOffXR(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var controlOn_off = await master.ReadCoilsAsync(modbusidbyte, CONTROL_ON_OFFXR, 1);
            
            return controlOn_off[0];
        }

        public async Task<bool> ReadControlOnOffXT(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var controlOn_off = await master.ReadHoldingRegistersAsync(modbusidbyte, CONROL_ON_OFFXT_READ, 1);

            if(controlOn_off[0] == 257)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ReadThawingXR(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var thawing = await master.ReadCoilsAsync(modbusidbyte, THAWING_XR, 1);
            return thawing[0];
        }
    }
}

