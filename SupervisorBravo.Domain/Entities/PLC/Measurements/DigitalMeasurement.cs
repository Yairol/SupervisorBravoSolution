using System.ComponentModel.DataAnnotations.Schema;

public class DigitalMeasurement : Measurement
{
    #region properties
    /// <summary>
    /// Valor de la medicion
    /// </summary>
    public bool MeasurementValue { get; set; }

    /// <summary>
/// Id de la variable que se mide
/// </summary>
    [ForeignKey(nameof(PLCDigitalVariableId))]
    public Guid PLCDigitalVariableId { get; set; }
    public PLCDigitalVariable PLCDigitalVariable { get; set; } = null!;

    #endregion

    #region builders
    public DigitalMeasurement() { }

    public DigitalMeasurement(bool value, PLCDigitalVariable variable, DateTime? time = null)
    : base(time)
    {
        MeasurementValue = value;
        PLCDigitalVariable = variable;
        PLCDigitalVariableId = variable.Id;
    }
    #endregion
}
    