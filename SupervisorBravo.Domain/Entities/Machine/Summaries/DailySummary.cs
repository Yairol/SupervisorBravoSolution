using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Domain.Entities.Machine.Summaries
{
    /// <summary>
    /// Clase para modelar el Resumen diario del funcionamiento de una maquina
    /// </summary>
    public class DailySummary : SummaryBase
    {
        /// <summary>
        /// Dia del que es el resumen
        /// </summary>
        public int Day { get; set; }
        /// <summary>
        /// Mes al que pertenece el resumen
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// Año al que pertenece el resumen
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Primera vez que la máquina se encendió en el día lógico.
        /// Null si nunca se encendió.
        /// </summary>
        public DateTime? FirstOnTime { get; set; }
    }
}
