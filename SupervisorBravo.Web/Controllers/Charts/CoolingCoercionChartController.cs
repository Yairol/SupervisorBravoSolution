using Microsoft.AspNetCore.Mvc;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Persistence.Abstracts.Temperatures;
using SupervisorBravo.Web.Models.DTOs;

namespace SupervisorBravo.Web.Controllers.Charts
{
    public class CoolingCoercionChartController : Controller
    {
        private readonly IDixellRepository _repository;

        public CoolingCoercionChartController(IDixellRepository repository)
        {
            _repository = repository;
        }


        [HttpGet, HttpPost]
        public async Task<IActionResult> CoolingCoercionChart(TemperatureFilterViewModel model)
        {

            if (model.StartDate == default && model.EndDate == default)
            {
                model.EndDate = DateTime.Now;
                model.StartDate = DateTime.Now.AddHours(-10);
            }
            var dixell = await _repository.GetDixellByRoomName<DixellXT111C>(model.DixellName);

            if (dixell != null)
            {
                model.Temperatures = await ((ITemperatureRepository)_repository).GetTemperaturesByDateRange(model.StartDate, model.EndDate, dixell.Id);
            }

            return View(model);
        }
    }
}
