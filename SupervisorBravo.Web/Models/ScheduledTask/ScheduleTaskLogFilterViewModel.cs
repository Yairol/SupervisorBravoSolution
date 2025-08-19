namespace SupervisorBravo.Web.ViewModels
{
    public class ScheduledTaskLogFilterViewModel
    {
        public List<ScheduledTaskLogViewModel> Logs { get; set; }
            = new List<ScheduledTaskLogViewModel> { new ScheduledTaskLogViewModel() };

        public string? SelectedOutcome { get; set; }
        public string? SelectedAction { get; set; }
        public string? SelectedDeviceName { get; set; }

        public List<string> AvailableOutcomes { get; set; } = new List<string>();
        public List<string> AvailableActions { get; set; } = new List<string>();
        public List<string> AvailableDeviceNames { get; set; } = new List<string>();

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
