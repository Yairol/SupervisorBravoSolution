using SupervisorBravo.Persistence.Abstracts;

public interface IPLCDeviceRepository : IRepository
{
    Task<List<PLCDevice>> GetAllAsync(bool includeVariables = false);
    Task<PLCDevice?> GetByIdAsync(Guid id, bool includeVariables = false);
    Task<List<PLCDevice>> GetByNameAsync(string namePart);
    Task AddAsync(PLCDevice device);
    Task UpdateAsync(PLCDevice device);
    Task DeleteAsync(Guid id);
}
