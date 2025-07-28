namespace SupervisorBravo.Web.ViewModels
{
    public class ScheduledTaskLogFilterViewModel
    {
        public List<ScheduledTaskLogViewModel> Logs { get; set; }

        public string? SelectedOutcome { get; set; }
        public string? SelectedAction { get; set; }
        public string? SelectedDeviceName { get; set; }

        public List<string> AvailableOutcomes { get; set; }
        public List<string> AvailableActions { get; set; }
        public List<string> AvailableDeviceNames { get; set; }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
