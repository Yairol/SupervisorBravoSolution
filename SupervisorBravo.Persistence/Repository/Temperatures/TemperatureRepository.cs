using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Domain.Entities.Temperatures;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using SupervisorBravo.Persistence.Helpers;

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

        public async Task<List<TemperatureAggregate>> GetTemperatureAggregates(DateTime startDate, DateTime endDate)
        {
            var dixells = await _context.Set<DixellBase>().ToListAsync();

            var result = new List<TemperatureAggregate>();

            foreach (var dixell in dixells)
            {
                var temps = await _context.Temperatures
                    .Where(t => t.MeasurementTime >= startDate &&
                                t.MeasurementTime <= endDate &&
                                t.DixellId == dixell.Id)
                    .OrderBy(t => t.MeasurementTime)
                    .ToListAsync();

                if (!temps.Any()) continue;

                var validTemps = temps.Where(t => t.TemperatureMeasurement != 0).ToList();

                TimeSpan offTime = TimeSpan.Zero;
                TimeSpan onTime = TimeSpan.Zero;
                TimeSpan controlTime = TimeSpan.Zero;
                TimeSpan disconnectTime = TimeSpan.Zero;

                for (int i = 1; i < temps.Count; i++)
                {
                    var actual = temps[i];
                    var anterior = temps[i - 1];
                    var deltaTime = actual.MeasurementTime - anterior.MeasurementTime;

                    if (anterior.DisconnectDixell)
                        disconnectTime += deltaTime;
                    else
                        onTime += deltaTime;

                    if (anterior.ControlEnable)
                        controlTime += deltaTime;

                    if (!anterior.On_OffDixell && !anterior.DisconnectDixell)
                        offTime += deltaTime;
                }

                var totalTime = onTime + disconnectTime;
                double avgControlTime = onTime.TotalMinutes > 0 ? (controlTime.TotalMinutes / onTime.TotalMinutes) * 100 : 0;
                double avgOffTime = totalTime.TotalMinutes > 0 ? (offTime.TotalMinutes / totalTime.TotalMinutes) * 100 : 0;
                double avgDisconnectTime = totalTime.TotalMinutes > 0 ? (disconnectTime.TotalMinutes / totalTime.TotalMinutes) * 100 : 0;

                result.Add(new TemperatureAggregate
                {
                    DixellId = dixell.Id,
                    DixellName = dixell.RoomName,
                    SetPoint = dixell.SetPoint,
                    AvgTemperature = validTemps.Any() ? Math.Round(validTemps.Average(t => t.TemperatureMeasurement), 2) : null,
                    MinTemperature = validTemps.Any() ? validTemps.Min(t => t.TemperatureMeasurement) : null,
                    MaxTemperature = validTemps.Any() ? validTemps.Max(t => t.TemperatureMeasurement) : null,
                    OffTime = offTime,
                    DisconnectTime = disconnectTime,
                    ControlTime = controlTime,
                    AvgControl = avgControlTime,
                    AvgOffTime = avgOffTime,
                    AvgDisconnectTime = avgDisconnectTime,
                    ModbusId = dixell.MoodbusId
                });
            }

            return result;
        }

        // 🔥 Nuevo método: obtener el último dato de temperatura de un Dixell
        public async Task<Temperature?> GetLastTemperatureByDixell(Guid dixellId)
        {
            var ctx = EnsureContext();

            return await ctx.Temperatures
                .Where(t => t.DixellId == dixellId)
                .OrderByDescending(t => t.MeasurementTime)
                .FirstOrDefaultAsync();
        }
    }
}
