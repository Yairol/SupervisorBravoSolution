namespace SupervisorBravo.Persistence.Helpers
{
    public class LogExportFilter
    {
        public string? Outcome { get; set; }
        public string? Action { get; set; }
        public string? DeviceName { get; set; }
        public DateTime? FromUtc { get; set; }
        public DateTime? ToUtc { get; set; }
    }
}
