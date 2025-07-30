public class PLCDigitalVariable : PLCVariable
{
    #region properties
    public int BitIndex { get; set; }
    public List<DigitalMeasurement> Measurements { get; set; } = new();

    #endregion

    #region builders
    public PLCDigitalVariable(string name, ushort address, bool isWritable, int bitIndex, PLCDevice device)
    : base(name, address, isWritable, device)
    {
        BitIndex = bitIndex;
    }
    #endregion
}
