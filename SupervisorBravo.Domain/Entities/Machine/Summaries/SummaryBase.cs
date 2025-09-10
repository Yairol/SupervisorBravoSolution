using SupervisorBravo.Domain.Entities.Common;
using SupervisorBravo.Domain.Entities.Machine.Machine;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Domain.Entities.Machine.Summaries
{
    /// <summary>
    /// Clase base para los Resumenes de funcionamiento
    /// </summary>
    public abstract class SummaryBase : Entity
    {
        /// <summary>
        /// Id de la Maquina a la que pertenece el resumen
        /// </summary>
        [ForeignKey(nameof(Machines))]
        public Guid MachineId { get; set; }
        /// <summary>
        /// maquina a la que pertenece el resumen
        /// </summary>
        public Machines Machine { get; set; } = new();
        /// <summary>
        /// Tiempo de actividad de la maquina en el resumen
        /// </summary>
        public TimeSpan OnTime { get; set; }
        /// <summary>
        /// Tiempo de inactividad de la maquina en el resumen
        /// </summary>
        public TimeSpan OffTime { get; set; }

        /// <summary>
        /// Última vez que este resumen fue actualizado por el WorkerService.
        /// Sirve para cargar solo mediciones nuevas.
        /// </summary>
        public DateTime LastUpdated { get; set; }

    }
}
