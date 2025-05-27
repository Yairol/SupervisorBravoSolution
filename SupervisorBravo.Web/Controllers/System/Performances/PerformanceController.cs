using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
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

            if(model.StartDateReport == default && model.EndDateReport == default){

                model.EndDateReport = DateTime.Now;
                model.StartDateReport = DateTime.Now.AddHours(-10);
            }

            await _dixellRepository.BeginTransaction();
            if (model.Room == "Refrigeracion")
            {

                var dixells = await _dixellRepository.GetAllDixells<DixellXR60CX>();

                foreach(var dixell in dixells)
                {
                    var temperaturas = await ((ITemperatureRepository)_dixellRepository).GetTemperaturesByDateRange(model.StartDateReport, model.EndDateReport, dixell.Id);
                    var temperaturasValidas = temperaturas.Where(t => t.TemperatureMeasurement != 0).ToList();
                    var temperaturasOrdenandas = temperaturas.OrderBy(t => t.MeasurementTime).ToList();

                    TimeSpan offTime = TimeSpan.Zero;
                    TimeSpan onTime = TimeSpan.Zero;
                    TimeSpan controlTime = TimeSpan.Zero;
                    TimeSpan disconnectTime = TimeSpan.Zero;

                    for(int i=1; i< temperaturasOrdenandas.Count; i++)
                    {
                        var actual = temperaturasOrdenandas[i];
                        var anterior = temperaturasOrdenandas[i - 1];

                        var deltaTime = actual.MeasurementTime - anterior.MeasurementTime;

                        if (anterior.DisconnectDixell)
                        {
                            disconnectTime += deltaTime;
                        }
                        else
                        {
                            onTime += deltaTime;
                        }
                        if (anterior.ControlEnable)
                        {
                            controlTime += deltaTime;
                        }
                        if (anterior.On_OffDixell == false)
                        {
                            offTime += deltaTime;
                        }
                    }

                    var totalTime = onTime + disconnectTime;
                    double avgControlTime = onTime.TotalMinutes > 0 ? (controlTime.TotalMinutes / onTime.TotalMinutes) * 100 : 0;
                    double avgOffTime = totalTime.TotalMinutes > 0 ? (offTime.TotalMinutes / totalTime.TotalMinutes) * 100 : 0;
                    double avgDisconnectTime = totalTime.TotalMinutes > 0 ? (disconnectTime.TotalMinutes / totalTime.TotalMinutes * 100) : 0;


                    var item = new DixellReporteItem
                    {
                        DixellName = dixell.RoomName,
                        SetPoint = dixell.SetPoint,
                        AvgTemperature = temperaturasValidas.Any()
                        ? Math.Round(temperaturasValidas.Average(t => t.TemperatureMeasurement), 2)
                        : null,
                        MaxTemperature = temperaturasValidas.Any()
                        ? temperaturasValidas.Max(t => t.TemperatureMeasurement)
                        : null,
                        MinTemperature = temperaturasValidas.Any() ? temperaturasValidas.Min(t => t.TemperatureMeasurement) : null,
                        OffTime = offTime,
                        DisconnectTime = disconnectTime,
                        ControlTime = controlTime,
                        AvgControl = avgControlTime,
                        AvgOffTime = avgOffTime,
                        AvgDisconnectTime = avgDisconnectTime,
                        
                    };
                    model.Reports.Add(item);
                }

            }else if(model.Room == "Coccion-Enfriamiento")
            {
                var dixells = await _dixellRepository.GetAllDixells<DixellXT111C>();

                foreach (var dixell in dixells)
                {
                    var temperaturas = await ((ITemperatureRepository)_dixellRepository).GetTemperaturesByDateRange(model.StartDateReport, model.EndDateReport, dixell.Id);
                    var temperaturasValidas = temperaturas.Where(t => t.TemperatureMeasurement != 0).ToList();
                    var temperaturasOrdenandas = temperaturas.OrderBy(t => t.MeasurementTime).ToList();

                    TimeSpan offTime = TimeSpan.Zero;
                    TimeSpan onTime = TimeSpan.Zero;
                    TimeSpan controlTime = TimeSpan.Zero;
                    TimeSpan disconnectTime = TimeSpan.Zero;

                    for (int i = 1; i < temperaturasOrdenandas.Count; i++)
                    {
                        var actual = temperaturasOrdenandas[i];
                        var anterior = temperaturasOrdenandas[i - 1];

                        var deltaTime = actual.MeasurementTime - anterior.MeasurementTime;

                        if (anterior.DisconnectDixell)
                        {
                            disconnectTime += deltaTime;
                        }
                        else
                        {
                            onTime += deltaTime;
                        }
                        if (anterior.ControlEnable)
                        {
                            controlTime += deltaTime;
                        }
                        if (anterior.On_OffDixell == false)
                        {
                            offTime += deltaTime;
                        }
                    }

                    var totalTime = onTime + disconnectTime;
                    double avgControlTime = onTime.TotalMinutes > 0 ? (controlTime.TotalMinutes / onTime.TotalMinutes) * 100 : 0;
                    double avgOffTime = totalTime.TotalMinutes > 0 ? (offTime.TotalMinutes / totalTime.TotalMinutes) * 100 : 0;
                    double avgDisconnectTime = totalTime.TotalMinutes > 0 ? (disconnectTime.TotalMinutes / totalTime.TotalMinutes * 100) : 0;


                    var item = new DixellReporteItem
                    {
                        DixellName = dixell.RoomName,
                        SetPoint = dixell.SetPoint,
                        AvgTemperature = temperaturasValidas.Any()
                        ? Math.Round(temperaturasValidas.Average(t => t.TemperatureMeasurement), 2)
                        : null,
                        MaxTemperature = temperaturasValidas.Any()
                        ? temperaturasValidas.Max(t => t.TemperatureMeasurement)
                        : null,
                        MinTemperature = temperaturasValidas.Any() ? temperaturasValidas.Min(t => t.TemperatureMeasurement) : null,
                        OffTime = offTime,
                        DisconnectTime = disconnectTime,
                        ControlTime = controlTime,
                        AvgControl = avgControlTime,
                        AvgOffTime = avgOffTime,
                        AvgDisconnectTime = avgDisconnectTime,

                    };
                    model.Reports.Add(item);
                }
            }
            await _dixellRepository.CommitTransaction();
            
            return View(model);
        }

    }
}
