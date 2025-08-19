namespace SupervisorBravo.Web.ViewModels
{
    public class ScheduledTaskLogViewModel
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Outcome { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid Id { get; set; }

    }
}
