using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Temperatures;
using SupervisorBravo.Persistence.Abstracts.Temperatures;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : ITemperatureRepository
    {
        public async Task<Temperature> CreateTemperature(double temperatureMeasurement, Guid dixellId)
        {
            Temperature temperature = new Temperature(temperatureMeasurement, DateTime.Now, dixellId);
            var dixell = await _context.Set<DixellBase>().FindAsync(dixellId);
            temperature.ControlEnable = dixell?.ControlON_OFF ?? false;

            await _context.AddAsync(temperature);
            return temperature;
        }

        public async Task<List<Temperature>> GetAllTemperaturesByDixell(DixellBase dixellBase)
        {
            return await _context.Temperatures.Where(t => t.DixellId == dixellBase.Id).OrderBy(t => t.MeasurementTime).ToListAsync();
        }

        public async Task<Temperature> GetTemperatureById(Guid id)
        {
            return await _context.Set<Temperature>().FindAsync(id);
        }

        public async Task<List<Temperature>> GetTemperaturesByDateRange(DateTime startDate, DateTime endDate, Guid dixellId)
        {
            return await _context.Temperatures.Where(t => t.MeasurementTime >= startDate && t.MeasurementTime <= endDate && t.DixellId == dixellId).OrderBy(t => t.MeasurementTime).ToListAsync();
        }
    }
}
