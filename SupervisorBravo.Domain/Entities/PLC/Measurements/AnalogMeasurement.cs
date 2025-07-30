using System.ComponentModel.DataAnnotations.Schema;

public class AnalogMeasurement : Measurement
{
    #region properties
    public double MeasurementValue { get; set; }

    // Relación inversa
    [ForeignKey(nameof(PLCAnalogVariable))]
    public Guid PLCAnalogVariableId { get; set; }
    public PLCAnalogVariable PLCAnalogVariable { get; set; }
    #endregion

    #region builders
    public AnalogMeasurement(double value, PLCAnalogVariable variable, DateTime? time = null)
    : base(time)
    {
        MeasurementValue = value;
        PLCAnalogVariable = variable;
        PLCAnalogVariableId = variable.Id;
    }
    #endregion
}
