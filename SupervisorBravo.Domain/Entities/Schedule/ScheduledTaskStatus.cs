using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Domain.Entities.Schedule
{
    public enum ScheduledTaskStatus
    {
        Pending,
        Executed,
        Failed,
        Cancelled

    }
}
