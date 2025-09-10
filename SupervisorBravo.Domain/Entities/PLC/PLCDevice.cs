using SupervisorBravo.Domain.Entities.Common;

public class PLCDevice : Entity
{
    #region properties
    /// <summary>
    /// Nombre del dispositivo
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Direccion modbus del dispositivo
    /// </summary>
    public byte ModbusId { get; set; }
    /// <summary>
    /// Direccion IP del dispositivo, por el momento solo para mostrar, pero puede ser utilizado para
    /// comunicarse con dispositivos via Modbus TCP
    /// </summary>
    public string? IpAddress { get; set; }

    // Relación directa con sus variables
    /// <summary>
    /// Variables del dispositivo
    /// </summary>
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
