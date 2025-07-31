using SupervisorBravo.Persistence.Abstracts;

public interface IAnalogMeasurementRepository : IRepository
{
    Task<List<AnalogMeasurement>> GetAllAnalogMeasurementsAsync();
    Task<AnalogMeasurement?> GetAnalogMeasurementByIdAsync(Guid id);
    Task AddAnalogMeasurementAsync(AnalogMeasurement measurement);
    Task UpdateAnalogMeasurementAsync(AnalogMeasurement measurement);
    Task DeleteAnalogMeasurementAsync(Guid id);
    Task<List<AnalogMeasurement>> GetAnalogMeasurementsByVariableIdAsync(Guid analogVariableId);
    Task<List<AnalogMeasurement>> GetAnalogMeasurementsByTimeRangeAsync(Guid analogVariableId, DateTime from, DateTime to);
}
