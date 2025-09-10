using SupervisorBravo.Persistence.Abstracts;

public interface IDigitalMeasurementRepository : IRepository

{
    /// <summary>
    /// Obtiene todas las Mediciones digitales del soporte de datos
    /// </summary>
    /// <returns></returns>
    Task<List<DigitalMeasurement>> GetAllDigitalMeasurementsAsync();
    /// <summary>
    /// Obtiene una Medicion digital por su Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<DigitalMeasurement?> GetDigitalMeasurementByIdAsync(Guid id);
    /// <summary>
    /// Agrega una medicion digital al soporte de datos
    /// </summary>
    /// <param name="measurement"></param>
    /// <returns></returns>
    Task AddDigitalMeasurementAsync(DigitalMeasurement measurement);
    /// <summary>
    /// Actualiza una medicion digital en el soporte de datos
    /// </summary>
    /// <param name="measurement"></param>
    /// <returns></returns>
    Task UpdateDigitalMeasurementAsync(DigitalMeasurement measurement);
    /// <summary>
    /// Elimina una Medicion Digital en el soporte de datos.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteDigitalMeasurementAsync(Guid id);
    /// <summary>
    /// Obtiene las mediciones digitales de una variable digital
    /// </summary>
    /// <param name="digitalVariableId"></param>
    /// <returns></returns>
    Task<List<DigitalMeasurement>> GetDigitalMeasurementsByVariableIdAsync(Guid digitalVariableId);
    /// <summary>
    /// Obtiene las mediciones digitales de una variable en un rango de tiempo determinado
    /// </summary>
    /// <param name="digitalVariableId"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    Task<List<DigitalMeasurement>> GetDigitalMeasurementsByTimeRangeAsync(Guid digitalVariableId, DateTime from, DateTime to);
}
