using SupervisorBravo.Domain.Entities.Common;
using SupervisorBravo.Domain.Entities.Machine.Summaries;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Domain.Entities.Machine.Machine
{
    /// <summary>
    /// Clase que modela una maquina
    /// </summary>
    public class Machines : Entity
    {
        /// <summary>
        /// Nombre de la maquina
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Modelo de la Maquina
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Id de la Variable que la registra
        /// </summary>
        [ForeignKey(nameof(PLCDigitalVariable))]
        public Guid VariableId { get; set; }
        /// <summary>
        /// Variable que lo registra
        /// </summary>
        public PLCDigitalVariable Variable { get; set; } = new();
        /// <summary>
        /// Id del PLC que lee la maquina
        /// </summary>
        public Guid PLCId { get; set; }
        /// <summary>
        /// PLC que lee la maquina
        /// </summary>
        public PLCDevice PLCDevice { get; set; } = new();
        /// <summary>
        /// Resumenes diarios de la maquina
        /// </summary>
        public List<DailySummary> DailySummaries { get; set; } = new();
        /// <summary>
        /// Resumenes mensuales de la maquina
        /// </summary>
        public List<MonthlySummary> MonthlySummaries { get; set; } = new();
        /// <summary>
        /// Resumenes anuales de la maquina
        /// </summary>
        public List<YearlySummary> YearlySummaries { get; set; } = new();
    }
}
