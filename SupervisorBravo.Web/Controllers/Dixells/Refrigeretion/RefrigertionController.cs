using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Dixells;

namespace SupervisorBravo.Web.Controllers.Dixells.Refrigeretion
{
    public class RefrigertionController : Controller
    {
        private readonly IDixellRepository _dixellRepository;

        public RefrigertionController(IDixellRepository dixellRepository)
        {
            _dixellRepository = dixellRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Dixells()
        {
            await _dixellRepository.BeginTransaction();

            var dixells = await _dixellRepository.GetAllDixells<DixellXR60CX>();
            var dixellsList = dixells.OrderBy(x => x.RoomName).ToList();
            await _dixellRepository.CommitTransaction();


            if (dixells == null)
            {
                return NotFound();
            }
            return View(dixellsList);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateDixell()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDixell(DixellXR60CX dixell)
        {
            await _dixellRepository.BeginTransaction();
            var newDixell = await _dixellRepository.CreateDixellXR60CX(dixell.RoomName, dixell.MoodbusId);
            await _dixellRepository.CommitTransaction();
            if (dixell == null)
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
            var dixell = await _dixellRepository.GetDixellByMoodbusId<DixellXR60CX>(id);
            await _dixellRepository.CommitTransaction();
            if (dixell == null)
                return NotFound();
            return View(dixell);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Tecnico")]
        public async Task<IActionResult> UpdateDixell(DixellXR60CX dixell)
        {
            await _dixellRepository.BeginTransaction();
            var dixellUpdate = await _dixellRepository.GetDixellById<DixellXR60CX>(dixell.Id);
            if (dixell == null) return NotFound();

            if (dixell.ControlON_OFF != dixellUpdate.ControlON_OFF)
            {
                dixellUpdate.ControlON_OFFWrite = true;
                dixellUpdate.ControlON_OFF = dixell.ControlON_OFF;
            }
            if ((dixell.Thawing != dixellUpdate.Thawing) && (dixellUpdate.ThawingWrite == false))
            {
                dixellUpdate.ThawingWrite = true;
                dixellUpdate.Thawing = dixell.Thawing;
            }

            if ((dixell.SetPoint != dixellUpdate.SetPoint) && (dixellUpdate.SetPointWrite == false))
            {
                dixellUpdate.SetPointWrite = true;
                dixellUpdate.SetPoint = dixell.SetPoint;
            }

            dixellUpdate.Id = dixell.Id;
            dixellUpdate.RoomName = dixell.RoomName;
            dixellUpdate.MoodbusId = dixell.MoodbusId;
            dixellUpdate.Thawing = dixell.Thawing;

            await _dixellRepository.UpdateDixell(dixellUpdate);

            await _dixellRepository.CommitTransaction();

            return RedirectToAction(nameof(Dixells));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDixell(int id)
        {
            await _dixellRepository.BeginTransaction();
            var dixell = await _dixellRepository.GetDixellByMoodbusId<DixellXR60CX>(id);
            await _dixellRepository.DeleteDixell<DixellXR60CX>(dixell.Id);
            await _dixellRepository.CommitTransaction();

            return RedirectToAction(nameof(Dixells));
        }
    }
}
