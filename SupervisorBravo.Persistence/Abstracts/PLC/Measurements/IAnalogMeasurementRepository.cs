using SupervisorBravo.Persistence.Abstracts;

public interface IAnalogMeasurementRepository : IRepository
{
    /// <summary>
    /// Obtiene todas las mediciones analogicas del soporte de datos
    /// </summary>
    /// <returns></returns>
    Task<List<AnalogMeasurement>> GetAllAnalogMeasurementsAsync();
    /// <summary>
    /// Obtiene una Medicion analogica por su id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<AnalogMeasurement?> GetAnalogMeasurementByIdAsync(Guid id);
    /// <summary>
    /// Agrega una medicion analogica al soporte de datos
    /// </summary>
    /// <param name="measurement"></param>
    /// <returns></returns>
    Task AddAnalogMeasurementAsync(AnalogMeasurement measurement);
    /// <summary>
    /// Actualiza una medicion analogica en el soporte de datos
    /// </summary>
    /// <param name="measurement"></param>
    /// <returns></returns>
    Task UpdateAnalogMeasurementAsync(AnalogMeasurement measurement);
    /// <summary>
    /// Elimina una Medicion Analogica del soporte de datos
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task DeleteAnalogMeasurementAsync(Guid id);
    /// <summary>
    /// Obtiene todas las Mediciones analogicas de una variable por el Id de la Variable
    /// </summary>
    /// <param name="analogVariableId"></param>
    /// <returns></returns>
    Task<List<AnalogMeasurement>> GetAnalogMeasurementsByVariableIdAsync(Guid analogVariableId);
    /// <summary>
    /// Obtiene las mediciones mediante una variable en el periodo de tiempo establecido
    /// </summary>
    /// <param name="analogVariableId"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    Task<List<AnalogMeasurement>> GetAnalogMeasurementsByTimeRangeAsync(Guid analogVariableId, DateTime from, DateTime to);
}
