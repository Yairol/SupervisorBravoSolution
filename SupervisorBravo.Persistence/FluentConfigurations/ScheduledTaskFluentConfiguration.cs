using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupervisorBravo.Domain.Entities.Common;
using SupervisorBravo.Domain.Entities.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence.FluentConfigurations
{
    internal class ScheduledTaskFluentConfiguration : IEntityTypeConfiguration<ScheduledTask>
    {
        public void Configure(EntityTypeBuilder<ScheduledTask> builder) 
        {
            builder.ToTable(nameof(ScheduledTask));
            builder.HasKey(t => t.Id);

            builder.HasOne(t=> t.Device)
                .WithMany()
                .HasForeignKey(t => t.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.ExecutionLog)
                .WithOne(log => log.Task)
                .HasForeignKey(log => log.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.Action)
                .HasConversion<string>();

            builder.Property(t => t.Status)
                .HasConversion<string>();

            builder.Property(t => t.Recurrence)
                .HasConversion<string>();

            builder.Property(t => t.RecurringDays)
                .HasColumnType("jsonb");

        }

    }
}
