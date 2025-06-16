using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupervisorBravo.Domain.Entities.Dixell;

namespace SupervisorBravo.Persistence.FluentConfigurations
{
    public class DixellXT111CFluentConfiguration : IEntityTypeConfiguration<DixellXT>
    {
        public void Configure(EntityTypeBuilder<DixellXT> builder)
        {
            builder.ToTable(nameof(DixellXT));
            builder.HasBaseType(typeof(DixellBase));
        }
    }
}
