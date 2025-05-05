using NModbus;
using NModbus.Serial;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Temperatures;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using SupervisorBravo.Persistence.Repository;
using SupervisorBravo.WorkerService.Utilities;
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
        const ushort CONTROL_ON_OFFXT = 1280;
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
                            

                            foreach (var dixell in dixells)
                            {
                                try
                                {
                                    //Lectura de temperaturas.
                                    double? temperatureValue = await ReadTemperature(master, dixell.MoodbusId).WaitAsync(TimeSpan.FromSeconds(5));
                                    if (temperatureValue != null)
                                        await ((ITemperatureRepository)repository).CreateTemperature(temperatureValue.Value, dixell.Id);
                                    await Task.Delay(1);

                                    //Logica de Lectura/Escritura de parametros del DixellXR60CX.

                                    await repository.PartialCommit();
                                    await Task.Delay(1);

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
                            await repository.CommitTransaction();

                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro al leer el puerto");
                    }

                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            }


        }

        public async Task<double> ReadTemperature(IModbusSerialMaster master, int modbusId)
        {
            var modbusidbyte = (byte)modbusId;
            var temperatureUshort = await master.ReadHoldingRegistersAsync(modbusidbyte, 256, 1);
            double temperatureValue = temperatureUshort[0] / 10.0;
            return temperatureValue;
        }
    }
}

