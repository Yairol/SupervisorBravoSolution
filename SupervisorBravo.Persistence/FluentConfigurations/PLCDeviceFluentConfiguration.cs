using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PLCDeviceFluentConfiguration : IEntityTypeConfiguration<PLCDevice>
{
    public void Configure(EntityTypeBuilder<PLCDevice> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(d => d.Variables)
            .WithOne(v => v.PLCDevice)
            .HasForeignKey(v => v.PLCDeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
