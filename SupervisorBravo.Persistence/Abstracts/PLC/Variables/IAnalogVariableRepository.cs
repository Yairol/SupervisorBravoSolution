using SupervisorBravo.Persistence.Abstracts;

public interface IAnalogVariableRepository : IRepository
{
    Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdAsync(Guid deviceId);
    Task<List<PLCAnalogVariable>> GetAllAnalogVariablesAsync();
    Task<PLCAnalogVariable?> GetAnalogVariableByIdAsync(Guid id);
    Task AddAnalogVariableAsync(PLCAnalogVariable variable);
    Task UpdateAnalogVariableAsync(PLCAnalogVariable variable);
    Task DeleteAnalogVariableAsync(Guid id);
    Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdWithMeasurementsAsync(Guid deviceId);
    Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdWithMeasurementsAsync(Guid deviceId, DateTime from, DateTime to);
    Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdWithLastMeasurementAsync(Guid deviceId);
}
