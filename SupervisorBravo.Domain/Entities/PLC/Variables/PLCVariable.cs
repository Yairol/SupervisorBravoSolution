using SupervisorBravo.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

public abstract class PLCVariable : Entity
{
    #region properties
    /// <summary>
    /// Nombre de la variable
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Dirección del registro del cual se va a leer la variable.
    /// </summary>
    public ushort Address { get; set; }
    /// <summary>
    /// Para determinar si la variable es de escritura o no.
    /// </summary>
    public bool IsWritable { get; set; }

    // FK para trazabilidad inversa al dispositivo
    /// <summary>
    /// Id del dispositivo al que pertenece la variable
    /// </summary>
    [ForeignKey(nameof(PLCDeviceId))]
    public Guid PLCDeviceId { get; set; }
    /// <summary>
    /// Dispositivo al que pertenece la variable
    /// </summary>
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
