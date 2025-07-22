using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SupervisorBravo.Persistence;
using SupervisorBravo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
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
            _logger.LogInformation("🔄 ScheduledTaskWorkerService iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var repository = scope.ServiceProvider.GetRequiredService<IScheduledTaskRepository>();
                        await repository.BeginTransaction();

                        var pendingTask = await repository.GetPendingTasks(DateTime.UtcNow);

                        if (pendingTask != null)
                        {
                            foreach (var task in pendingTask)
                            {
                                try
                                {
                                    var device = ((IDixellRepository)repository).GetDixellById<DixellBase>(task.DeviceId);
                                    if (device == null)
                                    {
                                        _logger.LogWarning($"⚠️ Dispositivo no encontrado para tarea {task.Id}.");
                                        continue;
                                    }

                                    if (task.ScheduledDateTime <= DateTime.UtcNow.AddMinutes(2))
                                    {
                                        await ExecuteScheduledTask(repository, task);

                                        if (!task.IsRecurring)
                                        {
                                            task.Status = ScheduledTaskStatus.Executed;
                                        }
                                        else
                                        {
                                            switch (task.Recurrence)
                                            {
                                                case RecurrenceType.Daily:
                                                    task.ScheduledDateTime = task.ScheduledDateTime.AddDays(1).ToUniversalTime();
                                                    break;
                                                case RecurrenceType.Weekly:
                                                    task.ScheduledDateTime = task.ScheduledDateTime.AddDays(7).ToUniversalTime();
                                                    break;
                                                case RecurrenceType.Monthly:
                                                    task.ScheduledDateTime = task.ScheduledDateTime.AddMonths(1).ToUniversalTime();
                                                    break;
                                                default:
                                                    throw new ArgumentException("Tipo de recurrencia inválida");
                                            }

                                            if (task.RecurrenceEndDate.HasValue &&
                                                task.ScheduledDateTime > task.RecurrenceEndDate.Value)
                                            {
                                                task.Status = ScheduledTaskStatus.Executed;
                                            }
                                        }

                                        await repository.UpdateTask(task);

                                        var successLog = new ScheduledTaskExecutionLog(task)
                                        {
                                            Timestamp = DateTime.UtcNow,
                                            Message = $"✅ Acción realizada: {task.Action} en dispositivo {task.DeviceId}",
                                            Outcome = ExecutionOutcome.Success,
                                            AttemptIndex = 1
                                        };
                                        await ((IScheduledTaskExecutionLogRepository)repository).AddLog(successLog);

                                        _logger.LogInformation($"✅ Tarea {task.Id} ejecutada para dispositivo {task.DeviceId}.");
                                    }
                                }
                                catch (Exception exTask)
                                {
                                    var failureLog = new ScheduledTaskExecutionLog(task)
                                    {
                                        Timestamp = DateTime.UtcNow,
                                        Message = $"❌ Error al ejecutar tarea {task.Id}: {exTask.Message}",
                                        Outcome = ExecutionOutcome.Failure,
                                        AttemptIndex = 1
                                    };
                                    await ((IScheduledTaskExecutionLogRepository)repository).AddLog(failureLog);

                                    _logger.LogError(exTask, $"❌ Error ejecutando tarea {task.Id}.");
                                }
                            }
                        }

                        await repository.CommitTransaction();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error procesando tareas programadas.");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task ExecuteScheduledTask(IScheduledTaskRepository context, ScheduledTask task)
        {
            var device = await ((IDixellRepository)context)
                .GetDixellById<DixellBase>(task.DeviceId);
            if (device == null)
            {
                _logger.LogWarning($"⚠️ Dispositivo {task.DeviceId} no encontrado para tarea {task.Id}.");
                return;
            }

            try
            {
                switch (task.Action)
                {
                    case ActionType.TurnOff:
                        device.ControlON_OFFWrite = true;
                        device.ControlON_OFF = false;
                        _logger.LogInformation($"🔌 [OFF] Bandera escrita en dispositivo {task.DeviceId}.");
                        break;

                    case ActionType.TurnOn:
                        device.ControlON_OFFWrite = true;
                        device.ControlON_OFF = true;
                        _logger.LogInformation($"⚡ [ON] Bandera escrita en dispositivo {task.DeviceId}.");
                        break;

                    case ActionType.ChangeSetPoint:
                        device.SetPointWrite = true;
                        if (task.SetPointValue.HasValue)
                        {
                            device.SetPoint = task.SetPointValue.Value;
                            _logger.LogInformation($"🎛️ SetPoint aplicado: {task.SetPointValue.Value}°C al dispositivo {task.DeviceId}.");
                        }
                        else
                        {
                            _logger.LogWarning($"⚠️ SetPoint no definido para tarea {task.Id}.");
                        }
                        break;

                    default:
                        _logger.LogWarning($"❓ Acción no reconocida para tarea {task.Action}");
                        break;
                }
            }
            catch (Exception exDevice)
            {
                _logger.LogError(exDevice, $"❌ Error físico al aplicar acción {task.Action} en dispositivo {task.DeviceId}.");
            }
        }
    }
}
