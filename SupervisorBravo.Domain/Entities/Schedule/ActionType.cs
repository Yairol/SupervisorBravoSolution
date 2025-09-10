namespace SupervisorBravo.Domain.Entities.Schedule
{
    /// <summary>
    /// Tipo de accion que va a realizar la tarea programada
    /// </summary>
    public enum ActionType
    {
        TurnOn,
        TurnOff,
        ChangeSetPoint
    }
}
