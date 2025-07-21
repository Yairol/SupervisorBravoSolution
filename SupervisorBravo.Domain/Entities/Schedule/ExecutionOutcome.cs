using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Domain.Entities.Schedule
{
    public enum ExecutionOutcome
    {
        Success,
        Failure,
        Timeout,
        Skipped,
        Unknown
    }
}
