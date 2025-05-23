namespace SupervisorBravo.Web.Models.DTOs
{
    public class DixellReporteViewModel
    {
        public string Sala { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public List<DixellReporteItem> Reportes { get; set; } = new();
    }

    public class DixellReporteItem
    {
        public string NombreDixell { get; set; }
        public double? SetPoint { get; set; }
        public double? TemperaturaPromedio { get; set; }
        public double? TemperaturaMinima { get; set; }
        public double? TemperaturaMaxima { get; set; }
        public double PorcientoControlando { get; set; }
        public TimeSpan TiempoControlando { get; set; }
        public TimeSpan TiempoDesconectado { get; set; }
        public TimeSpan TiempoApagado { get; set; }
    }

}
