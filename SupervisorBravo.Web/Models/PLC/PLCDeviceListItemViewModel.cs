namespace SupervisorBravo.Web.Models.PLC
{
    public class PLCDeviceListItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ModbusId { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }

}
