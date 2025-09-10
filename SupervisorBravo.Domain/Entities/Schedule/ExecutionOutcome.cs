namespace SupervisorBravo.Domain.Entities.Schedule
{
    /// <summary>
    /// Resultado de la ejecucion
    /// </summary>
    public enum ExecutionOutcome
    {
        Success,
        Failure,
        Timeout,
        Skipped,
        Unknown
    }
}
