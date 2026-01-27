using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupervisorBravo.Domain.Entities.Temperatures;

namespace SupervisorBravo.Persistence.FluentConfigurations
{
    /// <summary>
    /// Configuración para la creación de tablas y relaciones.
    /// </summary>
    internal class TemperatureFluentConfiguration : IEntityTypeConfiguration<Temperature>
    {
        public void Configure(EntityTypeBuilder<Temperature> builder)
        {
            builder.ToTable(nameof(Temperature));

            // Índice compuesto para optimizar consultas por dispositivo y rango de tiempo
            builder.HasIndex(t => new { t.DixellId, t.MeasurementTime })
                   .HasDatabaseName("IX_Temperatures_DixellId_MeasurementTime");
        }
    }
}
