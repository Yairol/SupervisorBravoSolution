using System.ComponentModel.DataAnnotations.Schema;

public class AnalogMeasurement : Measurement
{
    #region properties
    /// <summary>
    /// Valor de la medicion
    /// </summary>
    public double MeasurementValue { get; set; }
    /// <summary>
/// Id de la variable que se midio
/// </summary>
    [ForeignKey(nameof(PLCAnalogVariable))]
    public Guid PLCAnalogVariableId { get; set; }
    /// <summary>
    /// Variable que se mide, para relacion inversa
    /// </summary>
    public PLCAnalogVariable PLCAnalogVariable { get; set; } = null!;
    #endregion

    #region builders
    public AnalogMeasurement() { }
    public AnalogMeasurement(double value, PLCAnalogVariable variable, DateTime? time = null)
    : base(time)
    {
        MeasurementValue = value;
        PLCAnalogVariable = variable;
        PLCAnalogVariableId = variable.Id;
    }
    #endregion
}
