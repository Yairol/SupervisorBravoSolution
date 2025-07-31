using SupervisorBravo.Domain.Entities.Common;

public abstract class Measurement : Entity
{
    #region properties
    public DateTime MeasurementTime { get; set; }
    #endregion

    #region builders
    public Measurement() { }

    protected Measurement(DateTime? time = null)
    {
        MeasurementTime = time ?? DateTime.UtcNow;
    }
    #endregion
}
