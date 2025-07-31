namespace SupervisorBravo.Web.Models.Variable
{
    public class AdministrarVariablesViewModel
    {
        public Guid PlcId { get; set; }
        public string PlcName { get; set; } = string.Empty;
        public List<VariableDigitalDto> Digitales { get; set; } = new();
        public List<VariableAnalogicaDto> Analogicas { get; set; } = new();
    }

    public class VariableDigitalDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Address { get; set; }
        public int BitIndex { get; set; }
        public bool IsWritable { get; set; }
    }

    public class VariableAnalogicaDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Address { get; set; }
        public bool IsWritable { get; set; }
    }

}
