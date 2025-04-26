using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Temperatures;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : ITemperatureRepository
    {
        public async Task<Temperature> CreateTemperature(double temperatureMeasurement, Guid dixellId)
        {
            Temperature temperature = new Temperature(temperatureMeasurement, DateTime.Now, dixellId);
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

        public async Task<List<Temperature>> GetTemperaturesByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.Temperatures.Where(t => t.MeasurementTime >= startDate && t.MeasurementTime <= endDate).OrderBy(t => t.MeasurementTime).ToListAsync();
        }
    }
}
