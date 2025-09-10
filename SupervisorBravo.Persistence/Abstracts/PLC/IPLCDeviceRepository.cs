using SupervisorBravo.Persistence.Abstracts;

public interface IPLCDeviceRepository : IRepository
{
    /// <summary>
    /// Obtiene todos los dispositivos del soporte de datos
    /// </summary>
    /// <param name="includeVariables"></param>
    /// <returns></returns>
    Task<List<PLCDevice>> GetAllPLCDeviceAsync(bool includeVariables = false);
    /// <summary>
    /// Obtiene un dispositivo del soporte de datos por su id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="includeVariables"></param>
    /// <returns></returns>
    Task<PLCDevice?> GetPLCDeviceByIdAsync(Guid id, bool includeVariables = false);
    /// <summary>
    /// Obtiene un dispositivo por su nombre
    /// </summary>
    /// <param name="namePart"></param>
    /// <returns></returns>
    Task<List<PLCDevice>> GetPLCDeviceByNameAsync(string namePart);
    /// <summary>
    /// Agrega un dispositivo al soporte de datos
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    Task AddPLCDeviceAsync(PLCDevice device);
    /// <summary>
    /// Actualiza un dispositivo en el soporte de datos
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    Task UpdatePLCDeviceAsync(PLCDevice device);
    /// <summary>
    /// Elimina un dispositivo del soporte de datos
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeletePLCDeviceAsync(Guid id);
}
