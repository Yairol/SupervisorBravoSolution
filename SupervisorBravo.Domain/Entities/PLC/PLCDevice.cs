using SupervisorBravo.Domain.Entities.Common;

public class PLCDevice : Entity
{
    #region properties
    public string Name { get; set; } = string.Empty;
    public byte ModbusId { get; set; }
    public string? IpAddress { get; set; }

    // Relación directa con sus variables
    public List<PLCVariable> Variables { get; set; } = new();
    #endregion


    #region builders
    public PLCDevice() { }
    public PLCDevice(string name, byte modbusId, string? ipAddress = null)
    {
        Name = name;
        ModbusId = modbusId;
        IpAddress = ipAddress;
    }
    #endregion
}
