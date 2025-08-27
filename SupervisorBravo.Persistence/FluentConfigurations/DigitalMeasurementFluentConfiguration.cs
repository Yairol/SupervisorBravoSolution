using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DigitalMeasurementFluentConfiguration : IEntityTypeConfiguration<DigitalMeasurement>
{
    public void Configure(EntityTypeBuilder<DigitalMeasurement> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.MeasurementTime)
            .IsRequired();

        builder.Property(m => m.MeasurementValue)
            .IsRequired();

        builder.HasOne(m => m.PLCDigitalVariable)
            .WithMany(v => v.Measurements)
            .HasForeignKey(m => m.PLCDigitalVariableId);
    }
}
