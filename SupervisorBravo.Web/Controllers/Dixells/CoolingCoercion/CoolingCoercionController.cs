using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using SupervisorBravo.Web.Models.DTOs;

namespace SupervisorBravo.Web.Controllers.Dixells.CoolingCoercion
{
    public class CoolingCoercionController : Controller
    {
        private readonly IDixellRepository _dixellRepository;
        public CoolingCoercionController(IDixellRepository aplicationRepository)
        {
            _dixellRepository = aplicationRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Dixells()
        {
            await _dixellRepository.BeginTransaction();
            var model = new DixellXTListViewModel();
            var dixells = await _dixellRepository.GetAllDixellsWithoutTemperatures<DixellXT>();
            var dixellsList = dixells.OrderBy(x => x.MoodbusId).ToList();
            foreach (var device in dixellsList)
            {
                //Solo carga los ultimos 30 minutos
                device.temperatures = await ((ITemperatureRepository)_dixellRepository)
                    .GetTemperaturesByDateRange(DateTime.Now.AddMinutes(-5).ToUniversalTime(), DateTime.Now.ToUniversalTime(), device.Id);
                if (device.temperatures.Count == 0)
                {
                    device.temperatures = await ((ITemperatureRepository)_dixellRepository).GetAllTemperaturesByDixell(device);
                }
            }
            model.Devices = dixellsList.ToList();
            await _dixellRepository.CommitTransaction();
            //cambie a mvc
            if (dixellsList == null)
            {
                return NotFound();
            }
            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateDixell()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDixell(DixellXT dixell)
        {
            await _dixellRepository.BeginTransaction();
            var newDixell = await _dixellRepository.CreateDixellXT111C(dixell.RoomName, dixell.MoodbusId);
            await _dixellRepository.CommitTransaction();
            if (newDixell == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Dixells));
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Tecnico")]
        public async Task<IActionResult> UpdateDixell(int id)
        {
            await _dixellRepository.BeginTransaction();
            var dixell = await _dixellRepository.GetDixellByMoodbusId<DixellXT>(id);
            await _dixellRepository.CommitTransaction();
            if (dixell == null)
                return NotFound();
            return View(dixell);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Tecnico")]
        public async Task<IActionResult> UpdateDixell(DixellXT dixell)
        {
            await _dixellRepository.BeginTransaction();
            var dixellUpdate = await _dixellRepository.GetDixellById<DixellXT>(dixell.Id);
            if (dixell == null) return NotFound();

            if (dixell.ControlON_OFF != dixellUpdate.ControlON_OFF && dixellUpdate.ControlON_OFFWrite == false)
            {
                dixellUpdate.ControlON_OFFWrite = true;
                dixellUpdate.ControlON_OFF = dixell.ControlON_OFF;
            }

            if (dixell.SetPoint != dixellUpdate.SetPoint && dixellUpdate.SetPointWrite == false)
            {
                dixellUpdate.SetPointWrite = true;
                dixellUpdate.SetPoint = dixell.SetPoint;
            }

            dixellUpdate.Id = dixell.Id;
            dixellUpdate.RoomName = dixell.RoomName;
            dixellUpdate.SetPoint = dixell.SetPoint;
            dixellUpdate.MoodbusId = dixell.MoodbusId;
            dixellUpdate.ControlON_OFF = dixell.ControlON_OFF;

            await _dixellRepository.UpdateDixell(dixellUpdate);

            await _dixellRepository.CommitTransaction();

            return RedirectToAction(nameof(Dixells));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDixell(int id)
        {
            await _dixellRepository.BeginTransaction();
            var dixell = await _dixellRepository.GetDixellByMoodbusId<DixellXT>(id);
            await _dixellRepository.DeleteDixell<DixellXT>(dixell.Id);
            await _dixellRepository.CommitTransaction();

            return RedirectToAction(nameof(Dixells));
        }
    }
}
