using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SupervisorBravo.Domain.Entities.PLC.Variables;
using SupervisorBravo.Web.Models.PLC;
using SupervisorBravo.Web.Models.Variable;

namespace SupervisorBravo.Web.Controllers
{
    public class VariableController : Controller
    {
        private readonly IPLCDeviceRepository _plcDeviceRepository;

        public VariableController(IPLCDeviceRepository plcDeviceRepository)
        {
            _plcDeviceRepository = plcDeviceRepository;
        }

        public async Task<IActionResult> EstadoActual(Guid id)
        {
            await _plcDeviceRepository.BeginTransaction();
            var plc = await _plcDeviceRepository.GetPLCDeviceByIdAsync(id);
            if (plc == null) return NotFound();

            var digitales = await ((IDigitalVariableRepository)_plcDeviceRepository)
                .GetDigitalVariableByDeviceIdWithLastMeasurementAsync(id);

            var analogicas = await ((IAnalogVariableRepository)_plcDeviceRepository)
                .GetAnalogVariableByDeviceIdWithLastMeasurementAsync(id);


            var modelo = new EstadoActualPlcViewModel
            {
                PlcId = plc.Id,
                PlcName = plc.Name,
                VariablesDigitales = digitales.Select(v => new VariableDigitalEstadoDto
                {
                    Nombre = v.Name,
                    UltimoValor = v.Measurements?
        .OrderByDescending(m => m.MeasurementTime)
                    .FirstOrDefault()?.MeasurementValue,
                    FechaMuestreo = v.Measurements?
                    .OrderByDescending(m => m.MeasurementTime)
        .FirstOrDefault()?.MeasurementTime.ToLocalTime()
                }).ToList(),

                VariablesAnalogicas = analogicas.Select(v => new VariableAnalogicaEstadoDto
                {
                    Nombre = v.Name,
                    UltimoValor = v.Measurements?
                        .OrderByDescending(m => m.MeasurementTime)
                        .FirstOrDefault()?.MeasurementValue,
                    FechaMuestreo = v.Measurements?
                        .OrderByDescending(m => m.MeasurementTime)
                        .FirstOrDefault()?.MeasurementTime.ToLocalTime(),
                }).ToList()

            };
            await _plcDeviceRepository.CommitTransaction();
            return View(modelo);
        }


        [HttpGet]
        public async Task<IActionResult> Administrar(Guid id)
        {
            await _plcDeviceRepository.BeginTransaction();
            var plc = await _plcDeviceRepository.GetPLCDeviceByIdAsync(id);
            if (plc == null) return NotFound();

            var digitales = await ((IDigitalVariableRepository)_plcDeviceRepository).GetDigitalVariableByDeviceIdAsync(id);
            var analogicas = await ((IAnalogVariableRepository)_plcDeviceRepository).GetAnalogVariableByDeviceIdAsync(id);

            var model = new AdministrarVariablesViewModel
            {
                PlcId = plc.Id,
                PlcName = plc.Name,
                Digitales = digitales.Select(v => new VariableDigitalDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Address = v.Address,
                    BitIndex = v.BitIndex,
                    IsWritable = v.IsWritable
                }).ToList(),
                Analogicas = analogicas.Select(v => new VariableAnalogicaDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Address = v.Address,
                    IsWritable = v.IsWritable
                }).ToList()
            };
            await _plcDeviceRepository.CommitTransaction();
            return View(model);
        }
        [HttpGet]
        public IActionResult Create(Guid id)
        {
            var model = new VariableCreateViewModel
            {
                PlcId = id,
                Type = "Digital" // Default
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VariableCreateViewModel model)
        {
            await _plcDeviceRepository.BeginTransaction();

            if (!ModelState.IsValid)
                return View(model);

            var plc = await _plcDeviceRepository.GetPLCDeviceByIdAsync(model.PlcId);
            if (plc == null)
            {
                Console.WriteLine("No se encontró el PLC");
                Console.WriteLine(model.PlcId);
                return View(model);
            }

            if (model.Type == "Digital")
            {
                var digital = new PLCDigitalVariable
                {
                    Id = Guid.NewGuid(),
                    PLCDeviceId = model.PlcId,
                    Name = model.Name.Trim(),
                    Address = (ushort)model.Address,
                    IsWritable = model.IsWritable,
                    BitIndex = model.BitIndex ?? 0
                };

                await ((IDigitalVariableRepository)_plcDeviceRepository)
                    .AddDigitalVariableAsync(digital);
            }
            else // Analógica
            {
                // Convertir el valor del select en el enum
                HoldingDataType holdingType;
                if (!Enum.TryParse(model.AnalogHoldingType, true, out holdingType))
                {
                    holdingType = HoldingDataType.floating; // valor por defecto
                }

                var analogica = new PLCAnalogVariable
                {
                    Id = Guid.NewGuid(),
                    PLCDeviceId = model.PlcId,
                    Name = model.Name.Trim(),
                    Address = (ushort)model.Address,
                    IsWritable = model.IsWritable,
                    Type = holdingType,
                    ScaleFactor = model.ScaleFactor > 0 ? model.ScaleFactor : 1.0
                };

                await ((IAnalogVariableRepository)_plcDeviceRepository)
                    .AddAnalogVariableAsync(analogica);
            }

            await _plcDeviceRepository.CommitTransaction();
            return RedirectToAction("Administrar", new { id = model.PlcId });
        }

        [HttpPost]
        public async Task<IActionResult> DigitalDelete(Guid id)
        {
            await _plcDeviceRepository.BeginTransaction();
            var variable = await ((IDigitalVariableRepository)_plcDeviceRepository).GetDigitalVariableByIdAsync(id);

            if (variable == null)
                return NotFound();

            await ((IDigitalVariableRepository)_plcDeviceRepository).DeleteDigitalVariableAsync(id);
            await _plcDeviceRepository.CommitTransaction();

            return RedirectToAction("Administrar", new { id = variable.PLCDeviceId });
        }
        [HttpPost]
        public async Task<IActionResult> AnalogDelete(Guid id)
        {
            await _plcDeviceRepository.BeginTransaction();
            var variable = await ((IAnalogVariableRepository)_plcDeviceRepository).GetAnalogVariableByIdAsync(id);

            if (variable == null)
                return NotFound();

            await ((IAnalogVariableRepository)_plcDeviceRepository).DeleteAnalogVariableAsync(id);
            await _plcDeviceRepository.CommitTransaction();

            return RedirectToAction("Administrar", new { id = variable.PLCDeviceId });
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id)
        {
            await _plcDeviceRepository.BeginTransaction();
            var digitalRepo = (IDigitalVariableRepository)_plcDeviceRepository;
            var analogRepo = (IAnalogVariableRepository)_plcDeviceRepository;

            // Buscar Digital
            var digital = await digitalRepo.GetDigitalVariableByIdAsync(id);
            if (digital != null)
            {
                // Para variables digitales, igual inicializamos HoldingTypes por si en el futuro se reutiliza la vista
                ViewBag.HoldingTypes = new SelectList(
                    new[]
                    {
                new { Value = "integer", Text = "Integer" },
                new { Value = "floating", Text = "Floating" }
                    },
                    "Value",
                    "Text"
                );

                return View(new EditVariableViewModel
                {
                    Id = digital.Id,
                    PlcId = digital.PLCDeviceId,
                    Name = digital.Name,
                    Address = digital.Address,
                    IsWritable = digital.IsWritable,
                    BitIndex = digital.BitIndex,
                    IsDigital = true,
                    AnalogHoldingType = null,
                    ScaleFactor = 1.0
                });
            }

            // Buscar Analógica
            var analog = await analogRepo.GetAnalogVariableByIdAsync(id);
            if (analog != null)
            {
                // Preparar lista de opciones con el valor seleccionado actual
                ViewBag.HoldingTypes = new SelectList(
                    new[]
                    {
                new { Value = "integer", Text = "Integer" },
                new { Value = "floating", Text = "Floating" }
                    },
                    "Value",
                    "Text",
                    analog.Type.ToString().ToLower()
                );

                return View(new EditVariableViewModel
                {
                    Id = analog.Id,
                    PlcId = analog.PLCDeviceId,
                    Name = analog.Name,
                    Address = analog.Address,
                    IsWritable = analog.IsWritable,
                    BitIndex = null,
                    IsDigital = false,
                    AnalogHoldingType = analog.Type.ToString().ToLower(),
                    ScaleFactor = analog.ScaleFactor <= 0 ? 1.0 : analog.ScaleFactor
                });
            }

            await _plcDeviceRepository.CommitTransaction();
            return NotFound();
        }


        // POST: Variable/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditVariableViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _plcDeviceRepository.BeginTransaction();

            var digitalRepo = (IDigitalVariableRepository)_plcDeviceRepository;
            var analogRepo = (IAnalogVariableRepository)_plcDeviceRepository;

            if (model.IsDigital)
            {
                var entity = await digitalRepo.GetDigitalVariableByIdAsync(model.Id);
                if (entity == null)
                {
                    ModelState.AddModelError("", "Variable digital no encontrada.");
                    await _plcDeviceRepository.RollbackTransaction();
                    return View(model);
                }

                if (model.BitIndex is < 0 or > 7)
                {
                    ModelState.AddModelError(nameof(model.BitIndex), "El Bit Index debe estar entre 0 y 7.");
                    await _plcDeviceRepository.RollbackTransaction();
                    return View(model);
                }

                entity.Name = model.Name.Trim();
                entity.Address = (ushort)model.Address;
                entity.IsWritable = model.IsWritable;
                entity.BitIndex = model.BitIndex ?? 0;

                await digitalRepo.UpdateDigitalVariableAsync(entity);
            }
            else
            {
                var entity = await analogRepo.GetAnalogVariableByIdAsync(model.Id);
                if (entity == null)
                {
                    ModelState.AddModelError("", "Variable analógica no encontrada.");
                    await _plcDeviceRepository.RollbackTransaction();
                    return View(model);
                }

                if (!Enum.TryParse<HoldingDataType>(model.AnalogHoldingType ?? "floating", true, out var holdingType))
                    holdingType = HoldingDataType.floating;

                if (model.ScaleFactor <= 0)
                {
                    ModelState.AddModelError(nameof(model.ScaleFactor), "El factor de escala debe ser mayor que 0.");
                    await _plcDeviceRepository.RollbackTransaction();
                    return View(model);
                }

                entity.Name = model.Name.Trim();
                entity.Address = (ushort)model.Address;
                entity.IsWritable = model.IsWritable;
                entity.Type = holdingType;
                entity.ScaleFactor = model.ScaleFactor;

                await analogRepo.UpdateAnalogVariableAsync(entity);
            }

            await _plcDeviceRepository.CommitTransaction();
            return RedirectToAction("Administrar", new { id = model.PlcId });
        }



    }


}
