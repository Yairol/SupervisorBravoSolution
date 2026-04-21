using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupervisorBravo.Persistence.Abstracts.System;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SupervisorBravo.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DataAnalysesController : Controller
    {
        private readonly IDataAnalysesRepository _dataAnalyses;

        public DataAnalysesController(IDataAnalysesRepository dataAnalyses)
        {
            _dataAnalyses = dataAnalyses;
        }

        // Vista principal
        public async Task<IActionResult> Index()
        {
            await _dataAnalyses.BeginTransaction();
            var latestAnalysis = await _dataAnalyses.GetLatestAnalysisAsync();
            await _dataAnalyses.CommitTransaction();

            return View(latestAnalysis);
        }

        // Acción para crear un nuevo análisis (ejecuta la aplicación de consola)
        [HttpPost]
        public IActionResult CreateAnalysis()
        {
            try
            {
                var exePath = @"C:\Datos\Trabajo\Supervisor Bravo\DataAnalysis\SupervisorBravo.DataAnalysisProgram.exe";

                var processInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    UseShellExecute = true // abre como si fuera doble clic
                };

                Process.Start(processInfo);

                TempData["Message"] = "Aplicación de análisis iniciada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error al iniciar el análisis: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // Acción para exportar a Excel
        public async Task<IActionResult> ExportToExcel()
        {
            await _dataAnalyses.BeginTransaction();
            var latestAnalysis = await _dataAnalyses.GetLatestAnalysisAsync();
            await _dataAnalyses.CommitTransaction();

            if (latestAnalysis == null)
            {
                TempData["Message"] = "No hay análisis disponibles para exportar.";
                return RedirectToAction("Index");
            }

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var ws = workbook.Worksheets.Add("Último Análisis");

            ws.Cell(1, 1).Value = "Nombre";
            ws.Cell(1, 2).Value = "Media";
            ws.Cell(1, 3).Value = "Mediana";
            ws.Cell(1, 4).Value = "Desviación Estándar";
            ws.Cell(1, 5).Value = "Mínimo";
            ws.Cell(1, 6).Value = "Máximo";

            int row = 2;
            foreach (var item in latestAnalysis.AnalyzedItems)
            {
                ws.Cell(row, 1).Value = item.Name;

                // Formato con dos decimales
                ws.Cell(row, 2).Value = item.Mean;
                ws.Cell(row, 2).Style.NumberFormat.Format = "0.00";

                ws.Cell(row, 3).Value = item.Median;
                ws.Cell(row, 3).Style.NumberFormat.Format = "0.00";

                ws.Cell(row, 4).Value = item.StandardDeviation;
                ws.Cell(row, 4).Style.NumberFormat.Format = "0.00";

                ws.Cell(row, 5).Value = item.Minimum;
                ws.Cell(row, 5).Style.NumberFormat.Format = "0.00";

                ws.Cell(row, 6).Value = item.Maximum;
                ws.Cell(row, 6).Style.NumberFormat.Format = "0.00";

                // Colorear desviación estándar según nivel
                if (item.StandardDeviation >= 5) // ejemplo: alta
                {
                    ws.Cell(row, 4).Style.Font.FontColor = ClosedXML.Excel.XLColor.Red;
                }
                else if (item.StandardDeviation <= 2) // ejemplo: baja
                {
                    ws.Cell(row, 4).Style.Font.FontColor = ClosedXML.Excel.XLColor.Green;
                }
                else
                {
                    ws.Cell(row, 4).Style.Font.FontColor = ClosedXML.Excel.XLColor.Black;
                }

                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            var fileName = $"UltimoAnalisis_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
        }
    }
}
