using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupervisorBravo.Domain.Entities.Dixell;

namespace SupervisorBravo.Persistence.FluentConfigurations
{
    public class DixellXT111CFluentConfiguration : IEntityTypeConfiguration<DixellXT111C>
    {
        public void Configure(EntityTypeBuilder<DixellXT111C> builder)
        {
            builder.ToTable(nameof(DixellXT111C));
            builder.HasBaseType(typeof(DixellBase));
        }
    }
}
