namespace SupervisorBravo.Web.ViewModels
{
    public class ScheduledTaskLogViewModel
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string Outcome { get; set; }
        public string RoomName { get; set; }
        public string Message { get; set; }
        public Guid Id { get; set; }

    }
}
