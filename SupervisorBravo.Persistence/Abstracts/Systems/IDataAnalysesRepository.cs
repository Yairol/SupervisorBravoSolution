using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SupervisorBravo.Domain.Entities.Analysis;

namespace SupervisorBravo.Persistence.Abstracts.System
{
    /// <summary>
    /// Interfaz de repositorio para manejar los análisis de datos y sus resultados.
    /// Permite operaciones de consulta, inserción y obtención de historial.
    /// </summary>
    public interface IDataAnalysesRepository : IRepository
    {
        /// <summary>
        /// Obtiene todos los análisis realizados (incluyendo sus resultados).
        /// </summary>
        Task<List<DataAnalysis>> GetAllAnalysesAsync();

        /// <summary>
        /// Obtiene un análisis específico por su Id.
        /// </summary>
        Task<DataAnalysis?> GetAnalysisByIdAsync(Guid id);

        /// <summary>
        /// Obtiene el último análisis realizado (ordenado por fecha).
        /// </summary>
        Task<DataAnalysis?> GetLatestAnalysisAsync();

        /// <summary>
        /// Inserta un nuevo análisis con sus resultados asociados.
        /// </summary>
        Task AddAnalysisAsync(DataAnalysis analysis);

        /// <summary>
        /// Elimina un análisis por su Id (incluyendo sus resultados).
        /// </summary>
        Task DeleteAnalysisAsync(Guid id);


    }
}
