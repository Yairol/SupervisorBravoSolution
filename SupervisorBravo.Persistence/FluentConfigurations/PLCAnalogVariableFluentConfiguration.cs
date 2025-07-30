using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class PLCAnalogVariableFluentConfiguration : IEntityTypeConfiguration<PLCAnalogVariable>
{
    public void Configure(EntityTypeBuilder<PLCAnalogVariable> builder)
    {
        builder.HasBaseType<PLCVariable>();

        builder.HasMany(a => a.Measurements)
            .WithOne(m => m.PLCAnalogVariable)
            .HasForeignKey(m => m.PLCAnalogVariableId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
