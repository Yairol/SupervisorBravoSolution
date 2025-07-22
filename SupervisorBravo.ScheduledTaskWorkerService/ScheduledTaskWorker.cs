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

                                if (device == null)
                                {
                                    _logger.LogWarning($"⚠️ Dispositivo {task.DeviceId} no encontrado para tarea {task.Id}.");
                                    var warningLog = new ScheduledTaskExecutionLog(task)
                                    {
                                        Timestamp = DateTime.UtcNow,
                                        Outcome = ExecutionOutcome.Failure,
                                        AttemptIndex = 1,
                                        Message = $"⚠️ Dispositivo no encontrado para tarea {task.Id}."
                                    };
                                    await ((IScheduledTaskExecutionLogRepository)taskRepo).AddLog(warningLog);
                                    continue;
                                }

                                if (task.ScheduledDateTime <= DateTime.UtcNow.AddMinutes(2))
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
                                            RecurrenceType.Daily => task.ScheduledDateTime.AddDays(1).ToUniversalTime(),
                                            RecurrenceType.Weekly => task.ScheduledDateTime.AddDays(7).ToUniversalTime(),
                                            RecurrenceType.Monthly => task.ScheduledDateTime.AddMonths(1).ToUniversalTime(),
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
                                        Timestamp = DateTime.UtcNow,
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

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Ajustable
            }
        }

        private async Task ApplyActionAndPersistDevice(DixellBase device, ScheduledTask task, IDixellRepository dixellRepo)
        {
            try
            {
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
