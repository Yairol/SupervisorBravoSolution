using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class AnalogMeasurementFluentConfiguration : IEntityTypeConfiguration<AnalogMeasurement>
{
    public void Configure(EntityTypeBuilder<AnalogMeasurement> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.MeasurementTime)
            .IsRequired();

        builder.Property(m => m.MeasurementValue)
            .IsRequired();

        builder.HasOne(m => m.PLCAnalogVariable)
            .WithMany(v => v.Measurements)
            .HasForeignKey(m => m.PLCAnalogVariableId);
    }
}
