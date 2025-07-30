using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class PLCVariableFluentConfiguration : IEntityTypeConfiguration<PLCVariable>
{
    public void Configure(EntityTypeBuilder<PLCVariable> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(v => v.PLCDevice)
            .WithMany(d => d.Variables)
            .HasForeignKey(v => v.PLCDeviceId);

        builder.Property(v => v.Address)
            .IsRequired();

        builder.Property(v => v.IsWritable)
            .IsRequired();
    }
}
