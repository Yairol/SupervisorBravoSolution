using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupervisorBravo.Web.Models.PLC;

namespace SupervisorBravo.Web.Controllers;

public class PLCController : Controller
{
    private readonly IPLCDeviceRepository _plcRepository;

    public PLCController(IPLCDeviceRepository plcRepository)
    {
        _plcRepository = plcRepository;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(PLCDeviceViewModel model)
    {
        await _plcRepository.BeginTransaction();
        if (!ModelState.IsValid)
            return View(model);

        var plc = new PLCDevice
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            ModbusId = (byte)model.ModbusAddress,
            IpAddress = model.IPAddress
        };

        await _plcRepository.AddPLCDeviceAsync(plc);
        await _plcRepository.CommitTransaction();
        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        await _plcRepository.BeginTransaction();
        var devices = await _plcRepository.GetAllPLCDeviceAsync();

        var model = devices.Select(d => new PLCDeviceListItemViewModel
        {
            Id = d.Id,
            Name = d.Name,
            ModbusId = d.ModbusId.ToString(),
            IpAddress = d.IpAddress ?? string.Empty
        }).ToList();
        await _plcRepository.CommitTransaction();
        return View(model);
    }
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        await _plcRepository.BeginTransaction();
        var device = await _plcRepository.GetPLCDeviceByIdAsync(id);
        if (device is null) return NotFound();

        var model = new PLCDeviceEditViewModel
        {
            Id = device.Id,
            Name = device.Name,
            ModbusId = device.ModbusId,
            IpAddress = device.IpAddress ?? string.Empty
        };
        await _plcRepository.CommitTransaction();
        return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(PLCDeviceEditViewModel model)
    {
        await _plcRepository.BeginTransaction();
        if (!ModelState.IsValid)
            return View(model);

        var device = await _plcRepository.GetPLCDeviceByIdAsync(model.Id);
        if (device is null) return NotFound();

        device.Name = model.Name;
        device.ModbusId = (byte)model.ModbusId;
        device.IpAddress = model.IpAddress;

        await _plcRepository.UpdatePLCDeviceAsync(device);

        await _plcRepository.CommitTransaction();
        return RedirectToAction("Index");
    }
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _plcRepository.BeginTransaction();
        var device = await _plcRepository.GetPLCDeviceByIdAsync(id);
        if (device == null) return NotFound();

        await _plcRepository.DeletePLCDeviceAsync(id);
        await _plcRepository.CommitTransaction();
        return RedirectToAction("Index");
    }


}
