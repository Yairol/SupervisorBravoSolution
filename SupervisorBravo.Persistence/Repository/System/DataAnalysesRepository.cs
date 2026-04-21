using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Analysis;
using SupervisorBravo.Persistence.Abstracts.System;

namespace SupervisorBravo.Persistence.Repository
{
    /// <summary>
    /// Repositorio para manejar los análisis de datos y sus resultados.
    /// </summary>
    public partial class AplicationRepository : IDataAnalysesRepository
    {
        /// <summary>
        /// Inserta un nuevo análisis con sus resultados asociados.
        /// </summary>
        public async Task AddAnalysisAsync(DataAnalysis analysis)
        {
            var ctx = EnsureContext();
            await ctx.Set<DataAnalysis>().AddAsync(analysis);
        }

        /// <summary>
        /// Obtiene todos los análisis realizados (incluyendo sus resultados).
        /// </summary>
        public async Task<List<DataAnalysis>> GetAllAnalysesAsync()
        {
            var ctx = EnsureContext();
            return await ctx.Set<DataAnalysis>()
                            .Include(a => a.AnalyzedItems)
                            .ToListAsync();
        }

        /// <summary>
        /// Obtiene un análisis específico por su Id.
        /// </summary>
        public async Task<DataAnalysis?> GetAnalysisByIdAsync(Guid id)
        {
            var ctx = EnsureContext();
            return await ctx.Set<DataAnalysis>()
                            .Include(a => a.AnalyzedItems)
                            .FirstOrDefaultAsync(a => a.Id == id);
        }

        /// <summary>
        /// Obtiene el último análisis realizado (ordenado por fecha).
        /// </summary>
        public async Task<DataAnalysis?> GetLatestAnalysisAsync()
        {
            var ctx = EnsureContext();
            return await ctx.Set<DataAnalysis>()
                            .Include(a => a.AnalyzedItems)
                            .OrderByDescending(a => a.AnalysisDate)
                            .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Elimina un análisis por su Id (incluyendo sus resultados).
        /// </summary>
        public async Task DeleteAnalysisAsync(Guid id)
        {
            var ctx = EnsureContext();
            var analysis = await ctx.Set<DataAnalysis>().FindAsync(id);
            if (analysis is not null)
            {
                ctx.Remove(analysis);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
