using SupervisorBravo.Persistence.Abstracts;

public interface IDigitalVariableRepository : IRepository
{
    Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdAsync(Guid deviceId);
    Task<List<PLCDigitalVariable>> GetAllDigitalVariablesAsync();
    Task<PLCDigitalVariable?> GetDigitalVariableByIdAsync(Guid id);
    Task AddDigitalVariableAsync(PLCDigitalVariable variable);
    Task UpdateDigitalVariableAsync(PLCDigitalVariable variable);
    Task DeleteDigitalVariableAsync(Guid id);
    Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdWithMeasurementsAsync(Guid deviceId);
    Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdWithMeasurementsAsync(Guid deviceId, DateTime from, DateTime to);
    Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdWithLastMeasurementAsync(Guid deviceId);

}
