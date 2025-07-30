public class PLCAnalogVariable : PLCVariable
{
    #region properties
    public List<AnalogMeasurement> Measurements { get; set; } = new();
    #endregion

    #region builders
    public PLCAnalogVariable(string name, ushort address, bool isWritable, PLCDevice device)
        : base(name, address, isWritable, device)
    {
    }
    #endregion
}
