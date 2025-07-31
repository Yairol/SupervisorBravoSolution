using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> EstadoActual(Guid id)
        {
            await _plcDeviceRepository.BeginTransaction();
            var plc = await _plcDeviceRepository.GetPLCDeviceByIdAsync(id);
            if (plc == null) return NotFound();

            var modelo = new EstadoActualPlcViewModel
            {
                PlcId = plc.Id,
                PlcName = plc.Name,
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
                Console.WriteLine("No se encontro el PLC");
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
                await ((IDigitalVariableRepository)_plcDeviceRepository).AddDigitalVariableAsync(digital);
            }
            else
            {
                var analogica = new PLCAnalogVariable
                {
                    Id = Guid.NewGuid(),
                    PLCDeviceId = model.PlcId,
                    Name = model.Name.Trim(),
                    Address = (ushort)model.Address,
                    IsWritable = model.IsWritable
                };
                await ((IAnalogVariableRepository)_plcDeviceRepository).AddAnalogVariableAsync(analogica);
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

            await ((IDigitalVariableRepository) _plcDeviceRepository).DeleteDigitalVariableAsync(id);
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

    }
}
