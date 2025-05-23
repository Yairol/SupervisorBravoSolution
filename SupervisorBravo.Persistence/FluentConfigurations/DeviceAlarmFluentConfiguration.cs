using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupervisorBravo.Domain.Entities.System;

namespace SupervisorBravo.Persistence.FluentConfigurations
{
    internal class DeviceAlarmFluentConfiguration : IEntityTypeConfiguration<DeviceAlarm>
    {
        public void Configure(EntityTypeBuilder<DeviceAlarm> builder)
        {
            builder.ToTable(nameof(DeviceAlarm));

        }
    }
}
