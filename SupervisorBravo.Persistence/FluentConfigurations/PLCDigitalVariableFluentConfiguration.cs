using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PLCDigitalVariableFLuentConfiguration : IEntityTypeConfiguration<PLCDigitalVariable>
{
    public void Configure(EntityTypeBuilder<PLCDigitalVariable> builder)
    {
        builder.HasBaseType<PLCVariable>();

        builder.Property(v => v.BitIndex)
            .IsRequired();

        builder.HasMany(d => d.Measurements)
            .WithOne(m => m.PLCDigitalVariable)
            .HasForeignKey(m => m.PLCDigitalVariableId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
