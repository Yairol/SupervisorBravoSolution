using SupervisorBravo.Domain.Entities.Schedule;

public class ScheduledTaskListItemViewModel
{
    public Guid Id { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public ActionType Action { get; set; }
    public DateTime ScheduledDateTime { get; set; }
    public bool IsRecurring { get; set; }
    public double? SetPointValue { get; set; }
    public ScheduledTaskStatus? Status { get; set; }

}
