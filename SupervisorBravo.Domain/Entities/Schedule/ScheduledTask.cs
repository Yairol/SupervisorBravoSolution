using Newtonsoft.Json;
using SupervisorBravo.Domain.Entities.Common;
using SupervisorBravo.Domain.Entities.Dixell;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Domain.Entities.Schedule
{
    public class ScheduledTask : Entity
    {
        #region properties
        /// <summary>
        /// ForeignKey del dispositivo al que se refiere la tarea
        /// </summary>
        public Guid DeviceId { get; set; }
        /// <summary>
        /// Dispositivo al cual se le va a realizar la tarea
        /// </summary>
        [ForeignKey(nameof(DeviceId))]
        public virtual DixellBase Device { get; set; } = null!;

        /// <summary>
        /// hora de inicio
        /// </summary>
        public DateTime ScheduledDateTime { get; set; }
        //Acción Programada
        /// <summary>
        /// Tipo de accion a realizar
        /// </summary>
        public ActionType Action {  get; set; }
        /// <summary>
        /// Valor del SetPoint en caso de que se decida modificar
        /// </summary>
        public double? SetPointValue { get; set; }
        /// <summary>
        /// Para ver si la tarea se repite en el tiempo
        /// True-> Se repite
        /// False-> Solo se realizará una vez
        /// </summary>
        public bool IsRecurring { get; set; }
        /// <summary>
        /// Tipo de repetición
        /// Diaria, Semanal, Mensual
        /// </summary>
        public RecurrenceType? Recurrence {  get; set; }
        /// <summary>
        /// Dias específicos si es semanal
        /// </summary>
        [NotMapped]
        public List<DayOfWeek>? RecurringDays { get; set; }
        /// <summary>
        /// Hasta que dia repetir(Si aplica)
        /// </summary>
        public DateTime? RecurrenceEndDate { get; set; }
        
        //Estado de la Ejecución
        /// <summary>
        /// Estado de la tarea
        /// </summary>
        public ScheduledTaskStatus? Status { get; set; }
        /// <summary>
        /// Fecha de creación de la tarea
        /// </summary>
        public DateTime CreatedAt { get; set; }

        //Control de fallos y trazabilidad
        public virtual List<ScheduledTaskExecutionLog> ExecutionLog { get; set; }

        #endregion

        #region builders

        public ScheduledTask()
        {
            ExecutionLog = new();
            Status = ScheduledTaskStatus.Pending;
        }

        #endregion
        [Column("RecurringDays")]
        public string RecurringDaysSerialized
        {
            get => JsonConvert.SerializeObject(RecurringDays);
            set => RecurringDays = string.IsNullOrEmpty(value)
                ? new List<DayOfWeek>()
                : JsonConvert.DeserializeObject<List<DayOfWeek>>(value);
        }
    }
}
