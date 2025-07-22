namespace SupervisorBravo.Web.ViewModels
{
    public class LogExportFilterViewModel
    {
        public string? SelectedOutcome { get; set; }
        public string? SelectedAction { get; set; }
        public string? SelectedDeviceName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<string> AvailableOutcomes { get; set; } = new();
        public List<string> AvailableActions { get; set; } = new();
        public List<string> AvailableDeviceNames { get; set; } = new();
    }
}
