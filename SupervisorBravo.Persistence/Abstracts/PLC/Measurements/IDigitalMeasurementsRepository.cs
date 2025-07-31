using SupervisorBravo.Persistence.Abstracts;

public interface IDigitalMeasurementRepository : IRepository

{
    Task<List<DigitalMeasurement>> GetAllDigitalMeasurementsAsync();
    Task<DigitalMeasurement?> GetDigitalMeasurementByIdAsync(Guid id);
    Task AddDigitalMeasurementAsync(DigitalMeasurement measurement);
    Task UpdateDigitalMeasurementAsync(DigitalMeasurement measurement);
    Task DeleteDigitalMeasurementAsync(Guid id);
    Task<List<DigitalMeasurement>> GetDigitalMeasurementsByVariableIdAsync(Guid digitalVariableId);
    Task<List<DigitalMeasurement>> GetDigitalMeasurementsByTimeRangeAsync(Guid digitalVariableId, DateTime from, DateTime to);
}
