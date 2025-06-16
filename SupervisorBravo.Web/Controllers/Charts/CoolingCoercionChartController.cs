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

        [HttpGet]
        public async Task<IActionResult> CoolingCoercionChart(string dixellName)
        {
            await _repository.BeginTransaction();
            var model = new TemperatureFilterViewModel
            {
                DixellName = dixellName,
                StartDate = DateTime.Today.AddHours(-6),
                EndDate = DateTime.Today.AddHours(6)
            };

            var dixell = await _repository.GetDixellByRoomName<DixellXT>(model.DixellName);
            if (dixell != null)
            {
                model.Temperatures = await ((ITemperatureRepository)_repository)
                    .GetTemperaturesByDateRange(model.StartDate.ToUniversalTime(), model.EndDate.ToUniversalTime(), dixell.Id);
            }
            await _repository.CommitTransaction();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CoolingCoercionChart(TemperatureFilterViewModel model)
        {
            await _repository.BeginTransaction();
            if (model.StartDate == default && model.EndDate == default)
            {
                model.EndDate = DateTime.Today.AddHours(6);
                model.StartDate = DateTime.Today.AddHours(-6);
            }
            var dixell = await _repository.GetDixellByRoomName<DixellXT>(model.DixellName);

            if (dixell != null)
            {
                model.Temperatures = await ((ITemperatureRepository)_repository).GetTemperaturesByDateRange(model.StartDate.ToUniversalTime(), model.EndDate.ToUniversalTime(), dixell.Id);
            }
            await _repository.CommitTransaction();
            return View(model);
        }
    }
}
