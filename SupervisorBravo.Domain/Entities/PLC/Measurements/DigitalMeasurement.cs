using System.ComponentModel.DataAnnotations.Schema;

public class DigitalMeasurement : Measurement
{
    #region properties
    public bool MeasurementValue { get; set; }

    // Relación inversa
    [ForeignKey(nameof(PLCDigitalVariableId))]
    public Guid PLCDigitalVariableId { get; set; }
    public PLCDigitalVariable PLCDigitalVariable { get; set; }

    #endregion

    #region #builders
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
