using SupervisorBravo.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class PLCVariable : Entity
{
    #region properties
    public string Name { get; set; } = string.Empty;
    public ushort Address { get; set; }
    public bool IsWritable { get; set; }

    // FK para trazabilidad inversa al dispositivo
    [ForeignKey(nameof(PLCDeviceId))]
    public Guid PLCDeviceId { get; set; }
    public PLCDevice PLCDevice { get; set; } = null!;

    #endregion

    #region builders
    public PLCVariable() { }
    protected PLCVariable(string name, ushort address, bool isWritable, PLCDevice device)
    {
        Name = name;
        Address = address;
        IsWritable = isWritable;
        PLCDevice = device;
        PLCDeviceId = device.Id;
    }

    #endregion
}
