using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupervisorBravo.Domain.Entities.Dixell;

namespace SupervisorBravo.Persistence.FluentConfigurations
{
    /// <summary>
    /// Configuracion para la creacion de tablas y relaciones.
    /// </summary>
    internal class DixellXR60CFluentConfiguration : IEntityTypeConfiguration<DixellXR>
    {
        public void Configure(EntityTypeBuilder<DixellXR> builder)
        {
            builder.ToTable(nameof(DixellXR));
            builder.HasBaseType(typeof(DixellBase));


        }
    }
}
