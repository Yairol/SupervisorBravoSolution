using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Temperatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence.Abstracts.Temperatures
{
    /// <summary>
    /// Define las operraciones en la Bd con la Temperatura.
    /// </summary>
    public interface ITemperatureRepository : IRepository
    {
        /// <summary>
        /// Crea una nueva temperatura.
        /// </summary>
        /// <param name="temperatureMeasurement">Temperatura a crear.</param>
        /// <returns>La temperatura creada.</returns>
        Task<Temperature> CreateTemperature(double temperatureMeasurement, Guid dixellId);
        /// <summary>
        /// Obtiene todas las temperaturas.
        /// </summary>
        /// <returns>Lista de temperaturas.</returns>
        Task<List<Temperature>> GetAllTemperaturesByDixell(DixellBase dixell);
        /// <summary>
        /// Obtiene una temperatura por su id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Temperature> GetTemperatureById(Guid id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        Task<List<Temperature>> GetTemperaturesByDateRange(DateTime startDate, DateTime endDate);


    }
}
