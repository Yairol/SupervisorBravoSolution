using SupervisorBravo.Persistence.Abstracts;

public interface IPLCDeviceRepository : IRepository
{
    Task<List<PLCDevice>> GetAllPLCDeviceAsync(bool includeVariables = false);
    Task<PLCDevice?> GetPLCDeviceByIdAsync(Guid id, bool includeVariables = false);
    Task<List<PLCDevice>> GetPLCDeviceByNameAsync(string namePart);
    Task AddPLCDeviceAsync(PLCDevice device);
    Task UpdatePLCDeviceAsync(PLCDevice device);
    Task DeletePLCDeviceAsync(Guid id);
}
