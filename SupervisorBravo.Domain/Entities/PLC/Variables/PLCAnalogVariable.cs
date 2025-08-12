using SupervisorBravo.Domain.Entities.PLC.Variables;

public class PLCAnalogVariable : PLCVariable
{
    #region properties
    public List<AnalogMeasurement> Measurements { get; set; } = new();

    public HoldingDataType Type { get; set; }

    #endregion

    #region builders
    public PLCAnalogVariable() { }
    public PLCAnalogVariable(string name, ushort address, bool isWritable, PLCDevice device)
        : base(name, address, isWritable, device)
    {
    }


    #endregion
}
