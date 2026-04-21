using System;

namespace SupervisorBravo.Domain.Entities.Analysis
{
    /// <summary>
    /// Representa los resultados estadísticos de un dispositivo o sala
    /// dentro de un análisis de datos.
    /// </summary>
    public class AnalyzedData
    {
        /// <summary>
        /// Identificador único del registro de datos analizados.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Nombre del dispositivo o sala analizada.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Media aritmética de los valores.
        /// </summary>
        public double Mean { get; set; }
        /// <summary>
        /// Mediana de los valores.
        /// </summary>
        public double Median { get; set; }
        /// <summary>
        /// Valor mínimo encontrado.
        /// </summary>
        public double Minimum { get; set; }
        /// <summary>
        /// Valor máximo encontrado.
        /// </summary>
        public double Maximum { get; set; }
        /// <summary>
        /// Desviación estándar de los valores.
        /// </summary>
        public double StandardDeviation { get; set; }
        /// <summary>
        /// Cantidad de valores descartados por el método IQR.
        /// </summary>
        public int DiscardedByIQR { get; set; }
        /// <summary>
        /// Clave foránea que referencia al análisis padre.
        /// </summary>
        public Guid DataAnalysisId { get; set; }
        /// <summary>
        /// Relación con el análisis al que pertenece este resultado.
        /// </summary>
        public DataAnalysis DataAnalysis { get; set; }
    }
}
