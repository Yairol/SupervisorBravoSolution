using Microsoft.AspNetCore.Mvc;
using SupervisorBravo.Web.Models.Variable;
using System.Globalization;

namespace SupervisorBravo.Web.Controllers
{
    public class VariableChartController : Controller
    {
        private readonly IPLCDeviceRepository _plcRepository;

        public VariableChartController(IPLCDeviceRepository plcRepository)
        {
            _plcRepository = plcRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(Guid plcId)
        {
            await _plcRepository.BeginTransaction();

            var plc = await _plcRepository.GetPLCDeviceByIdAsync(plcId);
            if (plc is null)
                return NotFound();

            var analogVars = await ((IAnalogVariableRepository)_plcRepository)
                .GetAnalogVariableByDeviceIdAsync(plcId);
            var digitalVars = await ((IDigitalVariableRepository)_plcRepository)
                .GetDigitalVariableByDeviceIdAsync(plcId);

            var now = DateTime.Now;
            var model = new VariableChartViewModel
            {
                PlcId = plcId,
                PlcName = plc.Name,
                StartDate = now.AddHours(-4),
                EndDate = now,
                AnalogVariables = analogVars
                    .OrderBy(v => v.Name, StringComparer.Create(CultureInfo.CurrentCulture, true))
                    .Select(v => new VariableOption { Id = v.Id, Name = v.Name })
                    .ToList(),
                DigitalVariables = digitalVars
                    .OrderBy(v => v.Name, StringComparer.Create(CultureInfo.CurrentCulture, true))
                    .Select(v => new VariableOption { Id = v.Id, Name = v.Name })
                    .ToList(),
                AnalogSeries = new(),
                DigitalSeries = new()
            };

            await _plcRepository.CommitTransaction();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Plot(VariableChartViewModel model)
        {
            await _plcRepository.BeginTransaction();
            try
            {
                // Basic range validation
                if (!model.StartDate.HasValue || !model.EndDate.HasValue)
                {
                    ModelState.AddModelError(nameof(model.EndDate), "Date range is required.");
                }
                else if (model.StartDate >= model.EndDate)
                {
                    ModelState.AddModelError(nameof(model.EndDate), "End date must be later than start date.");
                }

                // Load PLC
                var plc = await _plcRepository.GetPLCDeviceByIdAsync(model.PlcId);
                if (plc is null)
                {
                    return NotFound();
                }

                // Rehydrate variable lists (for checkboxes)
                var analogVars = await ((IAnalogVariableRepository)_plcRepository)
                    .GetAnalogVariableByDeviceIdAsync(model.PlcId);
                var digitalVars = await ((IDigitalVariableRepository)_plcRepository)
                    .GetDigitalVariableByDeviceIdAsync(model.PlcId);

                model.PlcName = plc.Name;
                model.AnalogVariables = analogVars
                    .OrderBy(v => v.Name, StringComparer.Create(CultureInfo.CurrentCulture, true))
                    .Select(v => new VariableOption { Id = v.Id, Name = v.Name })
                    .ToList();
                model.DigitalVariables = digitalVars
                    .OrderBy(v => v.Name, StringComparer.Create(CultureInfo.CurrentCulture, true))
                    .Select(v => new VariableOption { Id = v.Id, Name = v.Name })
                    .ToList();

                if (!ModelState.IsValid)
                {
                    model.AnalogSeries = new();
                    model.DigitalSeries = new();
                    return View("Index", model);
                }

                // Normalize to UTC for repository filters
                var fromUtc = model.StartDate!.Value.Kind == DateTimeKind.Utc
                    ? model.StartDate.Value
                    : model.StartDate.Value.ToUniversalTime();
                var toUtc = model.EndDate!.Value.Kind == DateTimeKind.Utc
                    ? model.EndDate.Value
                    : model.EndDate.Value.ToUniversalTime();
                if (fromUtc > toUtc) (fromUtc, toUtc) = (toUtc, fromUtc);

                // Fetch variables including their measurements in the range
                var analogWithMeas = await ((IAnalogVariableRepository)_plcRepository)
                    .GetAnalogVariableByDeviceIdWithMeasurementsAsync(model.PlcId, fromUtc, toUtc);
                var digitalWithMeas = await ((IDigitalVariableRepository)_plcRepository)
                    .GetDigitalVariableByDeviceIdWithMeasurementsAsync(model.PlcId, fromUtc, toUtc);

                var colorAllocator = new DeterministicColorAllocator();

                // Build analog series
                model.AnalogSeries = model.SelectedAnalogIds
                    .Select(id =>
                    {
                        var v = analogWithMeas.FirstOrDefault(x => x.Id == id);
                        if (v is null) return null;

                        var color = colorAllocator.GetColor(v.Name);
                        var points = v.Measurements?
                            .OrderBy(m => m.MeasurementTime)
                            .Select(m => new Point<double>
                            {
                                Timestamp = (m.MeasurementTime.Kind == DateTimeKind.Utc
                                    ? m.MeasurementTime
                                    : DateTime.SpecifyKind(m.MeasurementTime, DateTimeKind.Utc)).ToLocalTime(),
                                Value = m.MeasurementValue
                            })
                            .ToList() ?? new List<Point<double>>();

                        return new Series<double>
                        {
                            Id = id,
                            Name = v.Name,
                            Color = color,
                            Points = points
                        };
                    })
                    .Where(s => s is not null)
                    .ToList()!;

                // Build digital series
                model.DigitalSeries = model.SelectedDigitalIds
                    .Select(id =>
                    {
                        var v = digitalWithMeas.FirstOrDefault(x => x.Id == id);
                        if (v is null) return null;

                        var color = colorAllocator.GetColor(v.Name);
                        var points = v.Measurements?
                            .OrderBy(m => m.MeasurementTime)
                            .Select(m => new Point<bool>
                            {
                                Timestamp = (m.MeasurementTime.Kind == DateTimeKind.Utc
                                    ? m.MeasurementTime
                                    : DateTime.SpecifyKind(m.MeasurementTime, DateTimeKind.Utc)).ToLocalTime(),
                                Value = m.MeasurementValue
                            })
                            .ToList() ?? new List<Point<bool>>();

                        return new Series<bool>
                        {
                            Id = id,
                            Name = v.Name,
                            Color = color,
                            Points = points
                        };
                    })
                    .Where(s => s is not null)
                    .ToList()!;

                return View("Index", model);
            }
            finally
            {
                await _plcRepository.CommitTransaction();
            }
        }


        private sealed class DeterministicColorAllocator
        {
            private static readonly string[] Palette = {
                "#1f77b4", "#ff7f0e", "#2ca02c", "#d62728",
                "#9467bd", "#8c564b", "#e377c2", "#7f7f7f",
                "#bcbd22", "#17becf", "#393b79", "#637939",
                "#8c6d31", "#843c39", "#7b4173", "#3182bd"
            };

            public string GetColor(string key)
            {
                if (string.IsNullOrWhiteSpace(key)) return Palette[0];
                unchecked
                {
                    int hash = 23;
                    foreach (var ch in key) hash = hash * 31 + ch;
                    return Palette[Math.Abs(hash) % Palette.Length];
                }
            }
        }
    }
}
