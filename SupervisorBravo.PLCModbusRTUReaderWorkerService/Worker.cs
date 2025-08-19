using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System;
using SupervisorBravo.Persistence.Repository;
using SupervisorBravo.PLCModbusRTUReaderWorkerService.Services;
using Microsoft.Extensions.DependencyInjection;
using SupervisorBravo.Domain.Entities.PLC.Variables;
using SupervisorBravo.Persistence.Abstracts.System;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IModbusReaderService _modbus;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IModbusReaderService modbus)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _modbus = modbus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🚀 Iniciando ciclo de lectura Modbus RTU...");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var plcRepository = scope.ServiceProvider.GetRequiredService<IPLCDeviceRepository>();
            var digitalRepo = (IDigitalVariableRepository)plcRepository;
            var analogRepo = (IAnalogVariableRepository)plcRepository;
            var digitalMedRepo = (IDigitalMeasurementRepository)plcRepository;
            var analogMedRepo = (IAnalogMeasurementRepository)plcRepository;
            var alarmRepo = (IAlarmRepository)plcRepository;


            try
            {
                await plcRepository.BeginTransaction();
                var dispositivos = await plcRepository.GetAllPLCDeviceAsync();

                foreach (var plc in dispositivos)
                {
                    _logger.LogInformation($"📡 PLC '{plc.Name}' (ModbusId: {plc.ModbusId})");

                    try
                    {
                        var digitales = await digitalRepo.GetDigitalVariableByDeviceIdAsync(plc.Id);
                        var analogicas = await analogRepo.GetAnalogVariableByDeviceIdAsync(plc.Id);

                        foreach (var digital in digitales)
                        {
                            try
                            {
                                var uValor = await _modbus.ReadHoldingRegisterAsync(plc.ModbusId, digital.Address);
                                bool valor = Helpers.GetBitValue(uValor[0], digital.BitIndex);
                                var medicion = new DigitalMeasurement
                                {
                                    Id = Guid.NewGuid(),
                                    PLCDigitalVariableId = digital.Id,
                                    MeasurementValue = valor,
                                    MeasurementTime = DateTime.UtcNow
                                };
                                await digitalMedRepo.AddDigitalMeasurementAsync(medicion);
                                _logger.LogInformation($"💡 Digital [{digital.Name}] Coil @ {digital.Address} = {(valor ? "ON" : "OFF")}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning($"⚠️ Error al leer digital '{digital.Name}': {ex.Message}");
                            }
                        }

                        foreach (var analogica in analogicas)
                        {
                            try
                            {
                                double valor;
                                var uValor = await _modbus.ReadHoldingRegisterAsync(plc.ModbusId, analogica.Address);
                                switch(analogica.Type)
                                {
                                    case HoldingDataType.floating:
                                        valor = Helpers.ConvertModbusToFloat(uValor[1], uValor[0]);
                                        break;
                                    case HoldingDataType.doubleinterger:
                                        valor = Helpers.ConvertModbusToInt(uValor[1], uValor[0]);
                                        break;
                                    case HoldingDataType.interger:
                                        valor = uValor[0];
                                        break;
                                    default:
                                        valor = 0;
                                        break;
                                }
                                valor *= analogica.ScaleFactor;
                                var medicion = new AnalogMeasurement
                                {
                                    Id = Guid.NewGuid(),
                                    PLCAnalogVariableId = analogica.Id,
                                    MeasurementValue = valor,
                                    MeasurementTime = DateTime.UtcNow
                                };
                                await analogMedRepo.AddAnalogMeasurementAsync(medicion);
                                _logger.LogInformation($"📈 Analógica [{analogica.Name}] Holding @ {analogica.Address} = {valor:F2}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning($"⚠️ Error al leer analógica '{analogica.Name}': {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Error de conexión Modbus. Se intentará nuevamente en el próximo ciclo.");


                        await alarmRepo.CreateAlarm(
                            "Error de desconexión del bus Modbus",
                            "Este error se produce debido a la desconexión del USB-RS485 del servidor. Para reconectar, inserte el adaptador USB-RS485 al servidor."
                        );
                    }
                }

                await plcRepository.CommitTransaction();
            }
            catch (Exception general)
            {
                _logger.LogError($"🔥 Error general en worker: {general.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        _logger.LogInformation("🛑 Worker detenido.");
    }
}
