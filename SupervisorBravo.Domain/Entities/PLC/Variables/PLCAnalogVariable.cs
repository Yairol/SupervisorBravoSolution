using SupervisorBravo.Domain.Entities.PLC.Variables;

public class PLCAnalogVariable : PLCVariable
{
    #region properties
    /// <summary>
    /// Lista de mediciones correspondiente a la variable en si
    /// </summary>
    public List<AnalogMeasurement> Measurements { get; set; } = new();
    /// <summary>
    /// Tipo de dato de la variable analogica
    /// </summary>
    public HoldingDataType Type { get; set; }
    /// <summary>
    /// Escala de conversion de las variables analogicas, por defecto 1.0
    /// </summary>
    public double ScaleFactor { get; set; }

    #endregion

    #region builders
    public PLCAnalogVariable()
    {
        Type = HoldingDataType.floating;
        ScaleFactor = 1.0;
    }
    public PLCAnalogVariable(string name, ushort address, bool isWritable, PLCDevice device)
        : base(name, address, isWritable, device)
    {
    }


    #endregion
}
