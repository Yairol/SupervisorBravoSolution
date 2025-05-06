using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupervisorBravo.Domain.Entities.Dixell;

namespace SupervisorBravo.Persistence.FluentConfigurations
{
    /// <summary>
    /// Configuracion para la creacion de tablas y relaciones.
    /// </summary>
    internal class DixellBaseFluentConfiguration : IEntityTypeConfiguration<DixellBase>
    {
        public void Configure(EntityTypeBuilder<DixellBase> builder)
        {
            builder.ToTable(nameof(DixellBase));
            builder.HasMany(t => t.temperatures).WithOne(p => p.Dixell).HasForeignKey(h => h.DixellId);
        }
    }
}
