using NModbus;
using NModbus.Serial;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Temperatures;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using SupervisorBravo.Persistence.Repository;
using System.IO.Ports;

namespace SupervisorBravo.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        const ushort TEMPERATURA = 256;


        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //using (SerialPort port = new SerialPort("COM9", 9600, Parity.None, 8, StopBits.One))
                //{
                //    try
                //    {
                //        port.Open();
                //        var factory = new ModbusFactory();
                //        IModbusSerialMaster master = factory.CreateRtuMaster(port);
                //        using (var scope = _scopeFactory.CreateScope())
                //        {
                //            var repository = scope.ServiceProvider.GetRequiredService<IDixellRepository>();
                //            await repository.BeginTransaction();
                //            var dixells = await repository.GetAllDixells<DixellXR60CX>();

                //            foreach (var dixell in dixells)
                //            {
                //                try
                //                {
                //                    var temperatureUshort = await master.ReadHoldingRegistersAsync(((byte)dixell.MoodbusId), TEMPERATURA, 1);
                //                    double temperatureValue = (65535.0 - temperatureUshort[0]) / 10;
                //                    await ((ITemperatureRepository)repository).CreateTemperature(temperatureValue, dixell.Id);
                //                    await repository.PartialCommit();
                //                }
                //                catch (Exception ex)
                //                {
                //                    _logger.LogError(ex, $"Error leyendo Dixell ID {dixell.MoodbusId}");
                //                }
                //            }

                //        }
                //    }catch(Exception ex)
                //    {
                //        _logger.LogError(ex, "Erro al leer el puerto");
                //    }

                //}
                
                using (var scope = _scopeFactory.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IDixellRepository>();
                    await repository.BeginTransaction();
                    var dixells = await repository.GetAllDixells<DixellXR60CX>();
                    Random rando = new Random();
                    foreach (var dixell in dixells)
                    {
                        await ((ITemperatureRepository)repository).CreateTemperature(rando.Next
                            (0, 20), dixell.Id);
                        await repository.PartialCommit();
                    }
                    await repository.CommitTransaction();

                }
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}
