using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using SupervisorBravo.Web.Models.DTOs;

namespace SupervisorBravo.Web.Controllers.Charts
{
    public class RefrigerationChartController : Controller
    {
        private readonly IDixellRepository _repository;

        public RefrigerationChartController(IDixellRepository repository)
        {
            _repository = repository;
        }


        [HttpGet, HttpPost]
        [Authorize]
        public async Task<IActionResult> RefrigerationChart(TemperatureFilterViewModel model)
        {

            if (model.StartDate == default && model.EndDate == default)
            {
                model.EndDate = DateTime.Now;
                model.StartDate = DateTime.Now.AddHours(-10);
            }
            var dixell = await _repository.GetDixellByRoomName<DixellXR60CX>(model.DixellName);

            if (dixell != null)
            {
                model.Temperatures = await ((ITemperatureRepository)_repository).GetTemperaturesByDateRange(model.StartDate, model.EndDate, dixell.Id);
            }

            return View(model);
        }


    }
}
