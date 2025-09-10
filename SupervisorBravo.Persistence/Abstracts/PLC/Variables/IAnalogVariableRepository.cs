using SupervisorBravo.Persistence.Abstracts;

public interface IAnalogVariableRepository : IRepository
{
    /// <summary>
    /// Obtiene las variables analogicas de un dispositivo por su Id
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdAsync(Guid deviceId);
    /// <summary>
    /// Obtiene todas las variables analogicas de un dispositivo del soporte de datos. Sin las mediciones
    /// </summary>
    /// <returns></returns>
    Task<List<PLCAnalogVariable>> GetAllAnalogVariablesAsync();
    /// <summary>
    /// Obtiene una variable analogica por su Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<PLCAnalogVariable?> GetAnalogVariableByIdAsync(Guid id);
    /// <summary>
    /// Agrega una variable Analogica al soporte de datos
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    Task AddAnalogVariableAsync(PLCAnalogVariable variable);
    /// <summary>
    /// Actualiza una variable analogica en el soporte de datos.
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    Task UpdateAnalogVariableAsync(PLCAnalogVariable variable);
    /// <summary>
    /// Elimina una variable analogica del soporte de datos
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAnalogVariableAsync(Guid id);
    /// <summary>
    /// Obtiene las variables analogicas de un dispositivos con las mediciones
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdWithMeasurementsAsync(Guid deviceId);
    /// <summary>
    /// Obtiene todas las variables analogicas de un dispositivo del soporte de datos con las mediciones en un rango de tiempo determinado.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdWithMeasurementsAsync(Guid deviceId, DateTime from, DateTime to);
    /// <summary>
    /// Obtiene las variables digitales de un dispositivo con la ultima medicion
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdWithLastMeasurementAsync(Guid deviceId);
}
