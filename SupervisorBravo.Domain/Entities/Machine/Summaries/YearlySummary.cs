using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Domain.Entities.Machine.Summaries
{
    /// <summary>
    /// Clase para modelar el funcionamiento anual de una maquina
    /// </summary>
    public class YearlySummary : SummaryBase
    {
        /// <summary>
        /// Año al que pertenece el resumen
        /// </summary>
        public int Year { get; set; }
    }
}