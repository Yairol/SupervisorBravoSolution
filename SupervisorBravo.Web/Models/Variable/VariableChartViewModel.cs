// Models/PLC/VariableChartViewModel.cs
namespace SupervisorBravo.Web.Models.Variable
{
    public class VariableOption
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class Point<T>
    {
        public DateTime Timestamp { get; set; }
        public T Value { get; set; } = default!;
    }

    public class Series<T>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = "#007bff";
        public List<Point<T>> Points { get; set; } = new();
    }

    public class VariableChartViewModel
    {
        public Guid PlcId { get; set; }
        public string PlcName { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<Guid> SelectedAnalogIds { get; set; } = new();
        public List<Guid> SelectedDigitalIds { get; set; } = new();

        public List<VariableOption> AnalogVariables { get; set; } = new();
        public List<VariableOption> DigitalVariables { get; set; } = new();

        public List<Series<double>> AnalogSeries { get; set; } = new();
        public List<Series<bool>> DigitalSeries { get; set; } = new();
    }
}
