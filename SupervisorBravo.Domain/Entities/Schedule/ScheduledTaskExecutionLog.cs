using SupervisorBravo.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorBravo.Domain.Entities.Schedule
{
    public class ScheduledTaskExecutionLog : Entity
    {

        #region Properties
        /// <summary>
        /// 🔗 Tarea que se está ejecutando
        /// </summary>
        public Guid TaskId { get; set; }
        [ForeignKey(nameof(TaskId))]
        public ScheduledTask Task { get; set; }

        /// <summary>
        /// 📅 Fecha y hora exacta del intento
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 📜 Mensaje log, resultado, diagnóstico
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 📊 Estado de la ejecución: Éxito, Falla, Timeout, Reintento
        /// </summary>
        public ExecutionOutcome Outcome { get; set; }

        /// <summary>
        /// 🔁 Número de intento (0 = primero)
        /// </summary>
        public int AttemptIndex { get; set; }
        #endregion

        public ScheduledTaskExecutionLog()
        {
            Task = null!;
            Message = String.Empty;
        }

        public ScheduledTaskExecutionLog(ScheduledTask task)
        {
            Task = task;
            TaskId = task.Id;
            Message = String.Empty;
        }

    }

}
