using System;
using System.Collections.Generic;

namespace SupervisorBravo.Domain.Entities.Analysis
{
    /// <summary>
    /// Representa una ejecución completa del análisis de datos.
    /// Contiene la fecha en que se realizó y la lista de resultados asociados.
    /// </summary>
    public class DataAnalysis
    {
        /// <summary>
        /// Identificador único del análisis.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Fecha en que se realizó el análisis.
        /// </summary>
        public DateTime AnalysisDate { get; set; }

        /// <summary>
        /// Relación 1:N con los datos analizados.
        /// Cada análisis puede tener múltiples resultados asociados.
        /// </summary>
        public ICollection<AnalyzedData> AnalyzedItems { get; set; } = new List<AnalyzedData>();
    }
}
