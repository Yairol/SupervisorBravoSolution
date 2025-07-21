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
    internal class ScheduledTaskExecutionLogFluentConfiguration : IEntityTypeConfiguration<ScheduledTaskExecutionLog>
    {
        public void Configure (EntityTypeBuilder<ScheduledTaskExecutionLog> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Task)
                  .WithMany(t => t.ExecutionLog)
                  .HasForeignKey(e => e.TaskId)

            .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.Outcome)
            .HasConversion<string>();

            builder.Property(e => e.Message)
                  .IsRequired();
        }
    }
}
