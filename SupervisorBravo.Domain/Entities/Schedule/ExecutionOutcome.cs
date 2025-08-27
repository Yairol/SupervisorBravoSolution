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
