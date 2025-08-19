using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Temperatures;
using SupervisorBravo.Persistence.Abstracts.Temperatures;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : ITemperatureRepository
    {
        public async Task<Temperature> CreateTemperature(double temperatureMeasurement, Guid dixellId, bool TemperatureReadTimeOut = false)
        {
            var ctx = EnsureContext();

            Temperature temperature = new Temperature(temperatureMeasurement, DateTime.UtcNow, dixellId);
            var dixell = await ctx.Set<DixellBase>().FindAsync(dixellId)
                         ?? throw new KeyNotFoundException($"No se encontró el Dixell con Id {dixellId}");

            if (TemperatureReadTimeOut)
                temperature.DisconnectDixell = true;
            else
                temperature.DisconnectDixell = false;

            if (dixell.ControlON_OFF && !temperature.DisconnectDixell)
                temperature.On_OffDixell = true;
            else
                temperature.On_OffDixell = false;

            if (dixell is DixellXR)
            {
                var dixellXR = await ctx.Set<DixellXR>().FindAsync(dixellId);
                temperature.ControlEnable = (dixell.SetPoint < temperatureMeasurement)
                                            && !temperature.DisconnectDixell
                                            && temperature.On_OffDixell;
                if (dixellXR != null)
                {
                    temperature.CoolingMeasurement = dixellXR.ControlON_OFF && !dixellXR.Thawing;
                    temperature.DefrostMeasurement = dixellXR.Thawing;
                }
            }

            if (dixell is DixellXT)
            {
                var dixellXT = await ctx.Set<DixellXT>().FindAsync(dixellId);
                temperature.ControlEnable = (dixell.SetPoint > temperatureMeasurement)
                                            && !temperature.DisconnectDixell
                                            && temperature.On_OffDixell;
                if (dixellXT != null)
                {
                    temperature.ElectroValveMeasurement = dixellXT.ElectroValve;
                }
            }

            // En pruebas
            temperature.SetPointMeasurement = dixell.SetPoint;

            await ctx.AddAsync(temperature);
            return temperature;
        }

        public async Task<List<Temperature>> GetAllTemperaturesByDixell(DixellBase dixellBase)
        {
            var ctx = EnsureContext();
            return await ctx.Temperatures
                .Where(t => t.DixellId == dixellBase.Id)
                .OrderBy(t => t.MeasurementTime)
                .ToListAsync();
        }

        public async Task<Temperature> GetTemperatureById(Guid id)
        {
            var ctx = EnsureContext();
            var temp = await ctx.Set<Temperature>().FindAsync(id)
                       ?? throw new KeyNotFoundException($"No se encontró la temperatura con Id {id}");
            return temp;
        }


        public async Task<List<Temperature>> GetTemperaturesByDateRange(DateTime startDate, DateTime endDate, Guid dixellId)
        {
            var ctx = EnsureContext();

            // Validación para que nunca sea menor el endDate que el StartDate
            if (startDate > endDate)
            {
                (startDate, endDate) = (endDate, startDate);
            }

            return await ctx.Temperatures
                .Where(t => t.MeasurementTime >= startDate &&
                            t.MeasurementTime <= endDate &&
                            t.DixellId == dixellId)
                .OrderBy(t => t.MeasurementTime)
                .ToListAsync();
        }
    }
}
