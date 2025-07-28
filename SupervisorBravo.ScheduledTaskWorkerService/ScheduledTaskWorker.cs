using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SupervisorBravo.Domain.Entities.Schedule;
using SupervisorBravo.Persistence.Abstracts.ScheduledTasks;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Temperatures;

namespace SupervisorBravo.WorkerService
{
    public class ScheduledTaskWorker : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<ScheduledTaskWorker> _logger;

        public ScheduledTaskWorker(IServiceProvider services, ILogger<ScheduledTaskWorker> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🟢 ScheduledTaskWorker iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var taskRepo = scope.ServiceProvider.GetRequiredService<IScheduledTaskRepository>();
                    await taskRepo.BeginTransaction();

                    var pendingTasks = await taskRepo.GetPendingTasks(DateTime.UtcNow);

                    if (pendingTasks != null)
                    {
                        foreach (var task in pendingTasks)
                        {
                            DixellBase? device = null;
                            try
                            {
                                using var deviceScope = _services.CreateScope();
                                var dixellRepo = deviceScope.ServiceProvider.GetRequiredService<IDixellRepository>();
                                await dixellRepo.BeginTransaction();
                                device = await dixellRepo.GetDixellById<DixellBase>(task.DeviceId);
                                await dixellRepo.CommitTransaction();

                                var roomName = device?.RoomName ?? "(desconocido)";

                                // 🚫 Si el dispositivo no existe
                                if (device == null)
                                {
                                    var warningLog = new ScheduledTaskExecutionLog(task)
                                    {
                                        Timestamp = DateTime.UtcNow,
                                        Outcome = ExecutionOutcome.Failure,
                                        AttemptIndex = 1,
                                        Message = $"⚠️ Dispositivo no encontrado para tarea {task.Id}."
                                    };
                                    await ((IScheduledTaskExecutionLogRepository)taskRepo).AddLog(warningLog);
                                    _logger.LogWarning(warningLog.Message);
                                    continue;
                                }

                                var scheduledTime = task.ScheduledDateTime;
                                var nowUtc = DateTime.UtcNow;
                                var marginUtc = scheduledTime.AddMinutes(5);

                                // ⏩ Si ya se venció el tiempo de ejecución permitido
                                if (nowUtc > marginUtc)
                                {
                                    if (!task.IsRecurring)
                                        task.Status = ScheduledTaskStatus.Cancelled;
                                    else
                                        task.ScheduledDateTime = task.Recurrence switch
                                        {
                                            RecurrenceType.Daily => scheduledTime.AddDays(1).ToUniversalTime(),
                                            RecurrenceType.Weekly => scheduledTime.AddDays(7).ToUniversalTime(),
                                            RecurrenceType.Monthly => scheduledTime.AddMonths(1).ToUniversalTime(),
                                            _ => throw new ArgumentException("Recurrencia inválida")
                                        };

                                    if (task.RecurrenceEndDate.HasValue &&
                                        task.ScheduledDateTime > task.RecurrenceEndDate.Value)
                                    {
                                        task.Status = ScheduledTaskStatus.Executed;
                                    }

                                    await taskRepo.UpdateTask(task);

                                    var skippedLog = new ScheduledTaskExecutionLog(task)
                                    {
                                        Timestamp = nowUtc,
                                        Outcome = ExecutionOutcome.Skipped,
                                        AttemptIndex = 1,
                                        Message = $"⏩ Tarea {task.Action} en dispositivo ({roomName}) fue saltada por exceder su tiempo de ejecución programado."
                                    };
                                    await ((IScheduledTaskExecutionLogRepository)taskRepo).AddLog(skippedLog);
                                    _logger.LogInformation(skippedLog.Message);
                                    continue;
                                }

                                // 🔌 Verificar si el dispositivo está desconectado según lecturas recientes
                                using var tempScope = _services.CreateScope();

                                var lecturaDesde = nowUtc.AddMinutes(-2);
                                var lecturaHasta = nowUtc;
                                await dixellRepo.BeginTransaction();
                                var muestreos = await ((ITemperatureRepository)dixellRepo).GetTemperaturesByDateRange(lecturaDesde, lecturaHasta, device.Id);
                                await dixellRepo.CommitTransaction();

                                var ultimoMuestreo = muestreos.LastOrDefault();

                                if (ultimoMuestreo == null || ultimoMuestreo.DisconnectDixell)
                                {
                                    var disconnectLog = new ScheduledTaskExecutionLog(task)
                                    {
                                        Timestamp = nowUtc,
                                        Outcome = ExecutionOutcome.Failure,
                                        AttemptIndex = 1,
                                        Message = $"❌ Dispositivo desconectado según último muestreo recibido ({roomName}). La tarea {task.Action} no se ejecutó."
                                    };
                                    await ((IScheduledTaskExecutionLogRepository)taskRepo).AddLog(disconnectLog);
                                    _logger.LogWarning(disconnectLog.Message);
                                    continue;
                                }

                                // ✅ Ejecutar tarea si está dentro del margen permitido
                                if (task.ScheduledDateTime <= nowUtc.AddMinutes(2))
                                {
                                    await ApplyActionAndPersistDevice(device, task, dixellRepo);

                                    if (!task.IsRecurring)
                                    {
                                        task.Status = ScheduledTaskStatus.Executed;
                                    }
                                    else
                                    {
                                        task.ScheduledDateTime = task.Recurrence switch
                                        {
                                            RecurrenceType.Daily => scheduledTime.AddDays(1).ToUniversalTime(),
                                            RecurrenceType.Weekly => scheduledTime.AddDays(7).ToUniversalTime(),
                                            RecurrenceType.Monthly => scheduledTime.AddMonths(1).ToUniversalTime(),
                                            _ => throw new ArgumentException("Recurrencia inválida")
                                        };

                                        if (task.RecurrenceEndDate.HasValue &&
                                            task.ScheduledDateTime > task.RecurrenceEndDate.Value)
                                        {
                                            task.Status = ScheduledTaskStatus.Executed;
                                        }
                                    }

                                    await taskRepo.UpdateTask(task);

                                    var successLog = new ScheduledTaskExecutionLog(task)
                                    {
                                        Timestamp = nowUtc,
                                        Outcome = ExecutionOutcome.Success,
                                        AttemptIndex = 1,
                                        Message = $"✅ Acción aplicada: {task.Action} en el dispositivo ({roomName})"
                                    };
                                    await ((IScheduledTaskExecutionLogRepository)taskRepo).AddLog(successLog);

                                    _logger.LogInformation(successLog.Message);
                                }
                            }
                            catch (Exception exTask)
                            {
                                var fallbackName = device?.RoomName ?? "(dispositivo desconocido)";
                                var failureLog = new ScheduledTaskExecutionLog(task)
                                {
                                    Timestamp = DateTime.UtcNow,
                                    Outcome = ExecutionOutcome.Failure,
                                    AttemptIndex = 1,
                                    Message = $"❌ Error ejecutando tarea {task.Id} en dispositivo {fallbackName}: {exTask.Message}"
                                };
                                await ((IScheduledTaskExecutionLogRepository)taskRepo).AddLog(failureLog);
                                _logger.LogError(exTask, failureLog.Message);
                            }
                        }
                    }

                    await taskRepo.CommitTransaction();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error general en ciclo de ejecución.");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // 🔁 Ciclo ajustable
            }
        }


        private async Task ApplyActionAndPersistDevice(DixellBase device, ScheduledTask task, IDixellRepository dixellRepo)
        {
            try
            {
                // 🖊 Aplicar acción al dispositivo según tipo
                switch (task.Action)
                {
                    case ActionType.TurnOff:
                        device.ControlON_OFFWrite = true;
                        device.ControlON_OFF = false;
                        break;

                    case ActionType.TurnOn:
                        device.ControlON_OFFWrite = true;
                        device.ControlON_OFF = true;
                        break;

                    case ActionType.ChangeSetPoint:
                        device.SetPointWrite = true;
                        if (task.SetPointValue.HasValue)
                        {
                            device.SetPoint = task.SetPointValue.Value;
                        }
                        else
                        {
                            _logger.LogWarning($"⚠️ SetPoint no definido en tarea {task.Id}.");
                        }
                        break;

                    default:
                        _logger.LogWarning($"❓ Acción no reconocida: {task.Action}");
                        return;
                }

                // 💾 Persistir estado actualizado del dispositivo en PostgreSQL (tiempos en UTC)
                await dixellRepo.BeginTransaction();
                await dixellRepo.UpdateDixell(device);
                await dixellRepo.CommitTransaction();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Error al aplicar y persistir acción {task.Action} en dispositivo {device.RoomName ?? "(sin nombre)"}.");
            }
        }
    }
}
