using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using SupervisorBravo.Web.Models.DTOs;
using System.Drawing;

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

                model.EndDateReport = DateTime.Today.AddHours(6);
                model.StartDateReport = DateTime.Today.AddHours(-6);
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
                        if (anterior.On_OffDixell == false && anterior.DisconnectDixell == false)
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
                        if (anterior.On_OffDixell == false && anterior.DisconnectDixell == false)
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

        [HttpPost]
        public async Task<IActionResult> ExportToExcel(DixellReporteViewModel model)
        {
            await Performance(model); // Rellenamos los datos como en la vista


            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Reporte");

            // Encabezados (igual que en la vista)
            string[] headers =
            {
        "Dispositivo", "SP", "AVG", "Min", "Max",
        model.Room == "Refrigeracion" ? "%Cool" : "Electro Válvula",
        "Tiempo Desconectado(horas)/% del intervalo",
        "Tiempo Apagado(Off)(horas)/% del intervalo"
    };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.Yellow);
                cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            int row = 2;
            foreach (var item in model.Reports)
            {
                worksheet.Cells[row, 1].Value = item.DixellName;
                worksheet.Cells[row, 2].Value = item.SetPoint;
                worksheet.Cells[row, 3].Value = item.AvgTemperature;
                worksheet.Cells[row, 4].Value = item.MinTemperature;
                worksheet.Cells[row, 5].Value = item.MaxTemperature;
                worksheet.Cells[row, 6].Value = $"{item.AvgControl:F1}%";
                worksheet.Cells[row, 7].Value = $"{item.DisconnectTime.TotalHours:F1}/{item.AvgDisconnectTime:F1}%";
                worksheet.Cells[row, 8].Value = $"{item.OffTime.TotalHours:F1}/{item.AvgOffTime:F1}%";

                for (int col = 1; col <= headers.Length; col++)
                {
                    worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                row++;
            }

            worksheet.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"ReporteDixell_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
