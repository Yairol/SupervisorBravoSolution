using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Domain.Entities.Machine.Summaries
{
    /// <summary>
    /// Clase para modelar el funcionamiento mensual de una maquina
    /// </summary>
    public class MonthlySummary : SummaryBase
    {
        /// <summary>
        /// Mes al que pertenece el resumen
        /// </summary>
        public int Month { get; set; }
        /// <summary>
        /// Año al que pertenece el resumen
        /// </summary>
        public int Year { get; set; }
    }
}
