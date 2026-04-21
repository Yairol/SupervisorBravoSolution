using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using SupervisorBravo.Domain.Entities.Analysis;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.System;

namespace SupervisorBravo.DataAnalysisProgram.Helpers
{
    public static class ProcessingHelper
    {
        public static async Task RunAnalysis(IDixellRepository dixellRepository, IDataAnalysesRepository dataAnalyses)
        {
            Console.WriteLine("Iniciando análisis estadístico de DixellXR...");

            // 1. Obtener dispositivos
            await dixellRepository.BeginTransaction();
            var dixells = await dixellRepository.GetAllDixells<DixellXR>();
            await dixellRepository.CommitTransaction();

            int total = dixells.Count;
            int procesados = 0;
            int totalValoresAnalizados = 0;

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Resumen Estadístico");

            ws.Cell(1, 1).Value = "Dispositivo";
            ws.Cell(1, 2).Value = "Media";
            ws.Cell(1, 3).Value = "Mediana";
            ws.Cell(1, 4).Value = "Desviación Estándar";
            ws.Cell(1, 5).Value = "Mínimo";
            ws.Cell(1, 6).Value = "Máximo";
            ws.Cell(1, 7).Value = "Descartados (IQR)";

            ws.Range(1, 1, 1, 7).Style.Font.Bold = true;
            ws.Range(1, 1, 1, 7).Style.Fill.BackgroundColor = XLColor.Yellow;

            int row = 2;

            var analysis = new DataAnalysis
            {
                Id = Guid.NewGuid(),
                AnalysisDate = DateTime.UtcNow,
                AnalyzedItems = new List<AnalyzedData>()
            };

            foreach (var dixell in dixells)
            {
                try
                {
                    var registros = dixell.temperatures
                        .Select(t => t.TemperatureMeasurement)
                        .ToList();

                    var registrosFiltrados = FiltrarOutliersIQR(registros);
                    int descartados = registros.Count - registrosFiltrados.Count;

                    if (registrosFiltrados.Any())
                    {
                        double media = registrosFiltrados.Average();
                        double mediana = CalcularMediana(registrosFiltrados);
                        double desviacion = CalcularDesviacion(registrosFiltrados, media);
                        double minimo = registrosFiltrados.Min();
                        double maximo = registrosFiltrados.Max();

                        ws.Cell(row, 1).Value = dixell.RoomName;
                        ws.Cell(row, 2).Value = media;
                        ws.Cell(row, 2).Style.NumberFormat.Format = "0.00";
                        ws.Cell(row, 3).Value = mediana;
                        ws.Cell(row, 3).Style.NumberFormat.Format = "0.00";
                        ws.Cell(row, 4).Value = desviacion;
                        ws.Cell(row, 4).Style.NumberFormat.Format = "0.00";
                        ws.Cell(row, 5).Value = minimo;
                        ws.Cell(row, 5).Style.NumberFormat.Format = "0.00";
                        ws.Cell(row, 6).Value = maximo;
                        ws.Cell(row, 6).Style.NumberFormat.Format = "0.00";
                        ws.Cell(row, 7).Value = descartados;

                        if (desviacion < 3)
                            ws.Row(row).Style.Fill.BackgroundColor = XLColor.LightGreen;
                        else if (desviacion > 10)
                            ws.Row(row).Style.Fill.BackgroundColor = XLColor.LightSalmon;

                        totalValoresAnalizados += registrosFiltrados.Count;

                        analysis.AnalyzedItems.Add(new AnalyzedData
                        {
                            Id = Guid.NewGuid(),
                            Name = dixell.RoomName,
                            Mean = media,
                            Median = mediana,
                            Minimum = minimo,
                            Maximum = maximo,
                            StandardDeviation = desviacion,
                            DiscardedByIQR = descartados,
                            DataAnalysisId = analysis.Id
                        });
                    }

                    Console.WriteLine($"✔ {dixell.RoomName}: descartados {descartados} valores por IQR.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error procesando dispositivo {dixell.RoomName}: {ex.Message}");
                }

                row++;
                procesados++;
                double porcentaje = (double)procesados / total * 100;
                Console.WriteLine($"Progreso: {procesados}/{total} ({porcentaje:F1}%)");
            }

            ws.Columns().AdjustToContents();

            var fileName = $"ResumenEstadistico_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            workbook.SaveAs(fileName);

            Console.WriteLine($"✅ Análisis completado. Archivo generado: {fileName}");
            Console.WriteLine($"📊 Total de valores analizados (tras filtrar outliers): {totalValoresAnalizados}");

            // Guardar en DB con flujo correcto: Begin → Add → Commit en DataAnalysesRepository
            await dataAnalyses.BeginTransaction();
            await dataAnalyses.AddAnalysisAsync(analysis);
            await dataAnalyses.CommitTransaction();

            Console.WriteLine("📥 Análisis guardado en la base de datos correctamente.");
        }

        private static double CalcularMediana(List<double> valores)
        {
            var ordenados = valores.OrderBy(v => v).ToList();
            int n = ordenados.Count;
            return n % 2 == 0
                ? (ordenados[n / 2 - 1] + ordenados[n / 2]) / 2.0
                : ordenados[n / 2];
        }

        private static double CalcularDesviacion(List<double> valores, double media)
        {
            double suma = valores.Sum(v => Math.Pow(v - media, 2));
            return Math.Sqrt(suma / valores.Count);
        }

        private static List<double> FiltrarOutliersIQR(List<double> valores)
        {
            if (valores == null || valores.Count == 0)
                return new List<double>();

            var ordenados = valores.OrderBy(x => x).ToList();
            int n = ordenados.Count;

            double Q1 = ordenados[(int)(0.25 * (n + 1)) - 1];
            double Q3 = ordenados[(int)(0.75 * (n + 1)) - 1];
            double IQR = Q3 - Q1;

            double limiteInferior = Q1 - 1.5 * IQR;
            double limiteSuperior = Q3 + 1.5 * IQR;

            return ordenados
                .Where(x => x >= limiteInferior && x <= limiteSuperior)
                .ToList();
        }
    }
}
