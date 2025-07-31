namespace SupervisorBravo.Web.Models.PLC
{
    public class PLCEstadoActualViewModel
    {
        public Guid PlcId { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte ModbusId { get; set; }
        public string? IpAddress { get; set; }
    }

}
