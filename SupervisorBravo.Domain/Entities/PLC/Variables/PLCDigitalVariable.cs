public class PLCDigitalVariable : PLCVariable
{
    #region properties
    /// <summary>
    /// Posicion en la trama de bits en la que se encuetra la variable en si
    /// Util para cuando el plc solo tiene registros tipo holding y se va a leer una variable digital
    /// de ahi
    /// </summary>
    public int BitIndex { get; set; }
    /// <summary>
    /// Lista de mediciones de la variable en si
    /// </summary>
    public List<DigitalMeasurement> Measurements { get; set; } = new();

    #endregion

    #region builders
    public PLCDigitalVariable() { }
    public PLCDigitalVariable(string name, ushort address, bool isWritable, int bitIndex, PLCDevice device)
    : base(name, address, isWritable, device)
    {
        BitIndex = bitIndex;
    }
    #endregion
}
