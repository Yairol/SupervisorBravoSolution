using Microsoft.AspNetCore.Mvc;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using SupervisorBravo.Web.Models.DTOs;

namespace SupervisorBravo.Web.Controllers.System.Performances
{
    public class PerformanceController : Controller
    {
        private readonly IDixellRepository _dixellRepository;


        public PerformanceController(IDixellRepository dixellRepository)
        {
            _dixellRepository = dixellRepository;
        }

        
        [HttpGet, HttpPost]
        public async Task<IActionResult> Performance(DixellReporteViewModel model)
        {

            if(model.FechaInicio == default && model.FechaFin == default){

                model.FechaFin = DateTime.Now;
                model.FechaInicio = DateTime.Now.AddHours(-10);
            }

            await _dixellRepository.BeginTransaction();
            if (model.Sala == "Refrigeracion")
            {

                var dixells = await _dixellRepository.GetAllDixells<DixellXR60CX>();

                foreach(var dixell in dixells)
                {
                    var temperaturas = await ((ITemperatureRepository)_dixellRepository).GetTemperaturesByDateRange(model.FechaInicio, model.FechaFin, dixell.Id);
                    var temperaturasValidas = temperaturas.Where(t => t.TemperatureMeasurement != 0).ToList();
                    var temperaturasOrdenandas = temperaturas.OrderBy(t => t.MeasurementTime).ToList();

                    TimeSpan offTime = TimeSpan.Zero;
                    TimeSpan onTime = TimeSpan.Zero;
                    TimeSpan controlTime = TimeSpan.Zero;

                    for(int i=1; i< temperaturasOrdenandas.Count; i++)
                    {
                        var actual = temperaturasOrdenandas[i];
                        var anterior = temperaturasOrdenandas[i - 1];

                        var deltaTime = actual.MeasurementTime - anterior.MeasurementTime;

                        if(anterior.TemperatureMeasurement == 0)
                        {
                            offTime += deltaTime;
                        }
                        else
                        {
                            onTime += deltaTime;
                        }
                        if(anterior.TemperatureMeasurement >= dixell.SetPoint && anterior.TemperatureMeasurement != 0)
                        {
                            controlTime += deltaTime;
                        }
                    }

                    var totalTime = model.FechaFin - model.FechaInicio;
                    double porcentajeControlando = onTime.TotalMinutes > 0 ? (controlTime.TotalMinutes / onTime.TotalMinutes) * 100 : 0;

                    var item = new DixellReporteItem
                    {
                        NombreDixell = dixell.RoomName,
                        SetPoint = dixell.SetPoint,
                        TemperaturaPromedio = temperaturasValidas.Any()
                        ? Math.Round(temperaturasValidas.Average(t => t.TemperatureMeasurement), 2)
                        : null,
                        TemperaturaMaxima = temperaturasValidas.Any()
                        ? temperaturasValidas.Max(t => t.TemperatureMeasurement)
                        : null,
                        TemperaturaMinima = temperaturasValidas.Any() ? temperaturasValidas.Min(t => t.TemperatureMeasurement) : null,
                        TiempoApagado = offTime,
                        TiempoDesconectado = onTime,
                        TiempoControlando = controlTime,
                        PorcientoControlando = porcentajeControlando,
                        
                    };
                    model.Reportes.Add(item);
                }

            }else if(model.Sala == "Coccion-Enfriamiento")
            {
                var dixells = await _dixellRepository.GetAllDixells<DixellXT111C>();

                foreach (var dixell in dixells)
                {
                    var temperaturas = await ((ITemperatureRepository)_dixellRepository).GetTemperaturesByDateRange(model.FechaInicio, model.FechaFin, dixell.Id);
                    var temperaturasValidas = temperaturas.Where(t => t.TemperatureMeasurement != 0).ToList();
                    var temperaturasOrdenandas = temperaturas.OrderBy(t => t.MeasurementTime).ToList();

                    TimeSpan offTime = TimeSpan.Zero;
                    TimeSpan onTime = TimeSpan.Zero;
                    TimeSpan controlTime = TimeSpan.Zero;

                    for (int i = 1; i < temperaturasOrdenandas.Count; i++)
                    {
                        var actual = temperaturasOrdenandas[i];
                        var anterior = temperaturasOrdenandas[i - 1];

                        var deltaTime = actual.MeasurementTime - anterior.MeasurementTime;

                        if (anterior.TemperatureMeasurement == 0)
                        {
                            offTime += deltaTime;
                        }
                        else
                        {
                            onTime += deltaTime;
                        }
                        if (anterior.TemperatureMeasurement >= dixell.SetPoint && anterior.TemperatureMeasurement != 0)
                        {
                            controlTime += deltaTime;
                        }
                    }

                    var totalTime = model.FechaFin - model.FechaInicio;
                    double porcentajeControlando = onTime.TotalMinutes > 0 ? (controlTime.TotalMinutes / onTime.TotalMinutes) * 100 : 0;

                    var item = new DixellReporteItem
                    {
                        NombreDixell = dixell.RoomName,
                        SetPoint = dixell.SetPoint,
                        TemperaturaPromedio = temperaturasValidas.Any()
                        ? Math.Round(temperaturasValidas.Average(t => t.TemperatureMeasurement), 2)
                        : null,
                        TemperaturaMaxima = temperaturasValidas.Any()
                        ? temperaturasValidas.Max(t => t.TemperatureMeasurement)
                        : null,
                        TemperaturaMinima = temperaturasValidas.Any() ? temperaturasValidas.Min(t => t.TemperatureMeasurement) : null,
                        TiempoApagado = offTime,
                        TiempoDesconectado = onTime,
                        TiempoControlando = controlTime,
                        PorcientoControlando = porcentajeControlando,

                    };
                    model.Reportes.Add(item);
                }
            }
            await _dixellRepository.CommitTransaction();
            return View(model);
        }

    }
}
