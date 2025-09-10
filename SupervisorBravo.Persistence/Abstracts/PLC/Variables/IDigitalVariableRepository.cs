using SupervisorBravo.Persistence.Abstracts;

public interface IDigitalVariableRepository : IRepository
{
    /// <summary>
    /// Obtiene todas las variables digitales de un dispositico sin las mediciones.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdAsync(Guid deviceId);
    /// <summary>
    /// Obtiene todas las variables digitales del soporte de datos
    /// </summary>
    /// <returns></returns>
    Task<List<PLCDigitalVariable>> GetAllDigitalVariablesAsync();
    /// <summary>
    /// Obtiene una variable digital por su Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<PLCDigitalVariable?> GetDigitalVariableByIdAsync(Guid id);
    /// <summary>
    /// Agrega una variable digital al soporte de datos
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    Task AddDigitalVariableAsync(PLCDigitalVariable variable);
    /// <summary>
    /// Actualiza una variable digital en el soporte de datos
    /// </summary>
    /// <param name="variable"></param>
    /// <returns></returns>
    Task UpdateDigitalVariableAsync(PLCDigitalVariable variable);
    /// <summary>
    /// Elimina una variable digital del soporte de datos
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteDigitalVariableAsync(Guid id);
    /// <summary>
    /// Obtiene todas las variables digitales de un dispositivo con las mediciones.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdWithMeasurementsAsync(Guid deviceId);
    /// <summary>
    /// Obtiene todas las variables digitales de un dispositivo con las mediciones en un intervalo determinado
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdWithMeasurementsAsync(Guid deviceId, DateTime from, DateTime to);
    /// <summary>
    /// Obtiene todas las variables digitales de un dispositivo con la ultima medicion.
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdWithLastMeasurementAsync(Guid deviceId);

}
