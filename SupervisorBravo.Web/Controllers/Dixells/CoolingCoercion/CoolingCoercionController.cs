using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Dixells;

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

            var dixells = await _dixellRepository.GetAllDixells<DixellXT111C>();

            await _dixellRepository.CommitTransaction();

            return View(dixells);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateDixell()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDixell(DixellXT111C dixell)
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
            var dixell = await _dixellRepository.GetDixellByMoodbusId<DixellXT111C>(id);
            await _dixellRepository.CommitTransaction();
            if (dixell == null)
                return NotFound();
            return View(dixell);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Tecnico")]
        public async Task<IActionResult> UpdateDixell(DixellXT111C dixell)
        {
            await _dixellRepository.BeginTransaction();
            var dixellUpdate = await _dixellRepository.GetDixellById<DixellXT111C>(dixell.Id);
            if (dixell == null) return NotFound();

            if (dixell.ControlON_OFF != dixellUpdate.ControlON_OFF && dixellUpdate.ControlON_OFFWrite == false)
            {
                dixellUpdate.ControlON_OFFWrite = true;
                dixellUpdate.ControlON_OFF = dixell.ControlON_OFF;
            }

            if (dixell.SetPoint != dixellUpdate.SetPoint && dixellUpdate.SetPointWrite == false)
            {
                dixellUpdate.ControlON_OFFWrite = true;
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
            var dixell = await _dixellRepository.GetDixellByMoodbusId<DixellXT111C>(id);
            await _dixellRepository.DeleteDixell<DixellXT111C>(dixell.Id);
            await _dixellRepository.CommitTransaction();

            return RedirectToAction(nameof(Dixells));
        }
    }
}
