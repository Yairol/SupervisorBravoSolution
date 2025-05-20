using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
