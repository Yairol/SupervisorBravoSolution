namespace SupervisorBravo.Web.Models.DTOs
{
    public class DixellReporteViewModel
    {
        public string Room { get; set; } = string.Empty;
        public DateTime StartDateReport { get; set; }
        public DateTime EndDateReport { get; set; }

        public List<DixellReporteItem> Reports { get; set; } = new();
    }

    public class DixellReporteItem
    {
        public string DixellName { get; set; } = string.Empty;
        public double? SetPoint { get; set; }
        public double? AvgTemperature { get; set; }
        public double? MinTemperature { get; set; }
        public double? MaxTemperature { get; set; }
        public double AvgControl { get; set; }
        public double AvgOffTime { get; set; }
        public double AvgDisconnectTime { get; set; }
        public TimeSpan ControlTime { get; set; }
        public TimeSpan DisconnectTime { get; set; }
        public TimeSpan OffTime { get; set; }
    }

}
