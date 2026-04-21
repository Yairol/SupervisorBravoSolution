using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupervisorBravo.Domain.Entities.Analysis;

namespace SupervisorBravo.Persistence.FluentConfigurations
{
    /// <summary>
    /// Configuración Fluent API para la entidad AnalyzedData.
    /// Define claves, propiedades y restricciones de los resultados estadísticos.
    /// </summary>
    public class AnalyzedDataConfiguration : IEntityTypeConfiguration<AnalyzedData>
    {
        public void Configure(EntityTypeBuilder<AnalyzedData> builder)
        {
            // Clave primaria
            builder.HasKey(d => d.Id);

            // Nombre del dispositivo/sala obligatorio y con longitud máxima
            builder.Property(d => d.Name)
                   .HasMaxLength(200)
                   .IsRequired();

            // Propiedades estadísticas obligatorias
            builder.Property(d => d.Mean).IsRequired();
            builder.Property(d => d.Median).IsRequired();
            builder.Property(d => d.Minimum).IsRequired();
            builder.Property(d => d.Maximum).IsRequired();
            builder.Property(d => d.StandardDeviation).IsRequired();
            builder.Property(d => d.DiscardedByIQR).IsRequired();

            // Relación con DataAnalysis (ya definida en la configuración de DataAnalysis)
            builder.HasOne(d => d.DataAnalysis)
                   .WithMany(a => a.AnalyzedItems)
                   .HasForeignKey(d => d.DataAnalysisId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
