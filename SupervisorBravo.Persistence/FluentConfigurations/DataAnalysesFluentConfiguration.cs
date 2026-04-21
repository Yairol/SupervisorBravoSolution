using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupervisorBravo.Domain.Entities.Analysis;

namespace SupervisorBravo.Persistence.FluentConfigurations
{
    /// <summary>
    /// Configuración Fluent API para la entidad DataAnalysis.
    /// Define claves, propiedades y relaciones con AnalyzedData.
    /// </summary>
    public class DataAnalysisConfiguration : IEntityTypeConfiguration<DataAnalysis>
    {
        public void Configure(EntityTypeBuilder<DataAnalysis> builder)
        {
            // Clave primaria
            builder.HasKey(a => a.Id);

            // Fecha del análisis obligatoria
            builder.Property(a => a.AnalysisDate)
                   .IsRequired();

            // Relación 1:N con AnalyzedData
            builder.HasMany(a => a.AnalyzedItems)
                   .WithOne(d => d.DataAnalysis)
                   .HasForeignKey(d => d.DataAnalysisId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
