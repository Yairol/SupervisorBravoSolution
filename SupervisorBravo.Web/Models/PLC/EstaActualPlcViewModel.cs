namespace SupervisorBravo.Web.Models.PLC
{
    public class EstadoActualPlcViewModel
    {
        public Guid PlcId { get; set; }
        public string PlcName { get; set; } = string.Empty;

        public List<VariableDigitalEstadoDto> VariablesDigitales { get; set; } = new();
        public List<VariableAnalogicaEstadoDto> VariablesAnalogicas { get; set; } = new();
    }

    public class VariableDigitalEstadoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public bool? UltimoValor { get; set; }
        public DateTime? FechaMuestreo { get; set; }
    }

    public class VariableAnalogicaEstadoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public double? UltimoValor { get; set; }
        public DateTime? FechaMuestreo { get; set; }
    }


}
