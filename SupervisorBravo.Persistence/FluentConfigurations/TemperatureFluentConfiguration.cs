using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupervisorBravo.Domain.Entities.Temperatures;

namespace SupervisorBravo.Persistence.FluentConfigurations
{
    /// <summary>
    /// Configuracion para la creacion de tablas y relaciones.
    /// </summary>
    internal class TemperatureFluentConfiguration : IEntityTypeConfiguration<Temperature>
    {
        public void Configure(EntityTypeBuilder<Temperature> builder)
        {
            builder.ToTable(nameof(Temperature));
        }
    }
}
