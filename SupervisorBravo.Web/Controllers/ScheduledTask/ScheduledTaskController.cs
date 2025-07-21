using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using SupervisorBravo.Domain.Entities.Schedule;
using SupervisorBravo.Persistence.Abstracts.Dixells;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.ScheduledTasks;

public class ScheduledTaskController : Controller
{
    private readonly IScheduledTaskRepository _repository;

    public ScheduledTaskController(IScheduledTaskRepository deviceRepo)
    {
        _repository = deviceRepo;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await _repository.BeginTransaction();
        var devices = await ((IDixellRepository)_repository).GetAllDixellsWithoutTemperatures<DixellXR>(); // Filtrar por tipo
        var viewModel = new ScheduleTaskViewModel
        {
            ScheduledDateTime = DateTime.Now.AddHours(1),
            Devices = devices.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.RoomName }).ToList(),
            ActionOptions = Enum.GetValues(typeof(ActionType))
                                .Cast<ActionType>()
                                .Select(a => new SelectListItem { Value = a.ToString(), Text = a.ToString() })
                                .ToList()
        };
        await _repository.CommitTransaction();
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ScheduleTaskViewModel model)
    {
        // Verifica si ya hay una transacción activa
        bool transaccionIniciadaInternamente = !_repository.IsInTransaction;

        if (transaccionIniciadaInternamente)
        {
            await _repository.BeginTransaction();
        }

        // Limpieza de campos que no deben validarse
        ModelState.Remove(nameof(ScheduleTaskViewModel.Devices));
        ModelState.Remove(nameof(ScheduleTaskViewModel.ActionOptions));

        // Si el modelo no es válido, recarga combos y vuelve a la vista
        if (!ModelState.IsValid)
        {
            foreach (var kv in ModelState)
            {
                foreach (var error in kv.Value.Errors)
                {
                    Console.WriteLine($"Campo {kv.Key}: {error.ErrorMessage}");
                }
            }

            model.Devices = await LoadDevicesAsync();
            model.ActionOptions = LoadActions();

            if (transaccionIniciadaInternamente)
                await _repository.CommitTransaction();

            return View(model);
        }

        // Crear la entidad ScheduledTask con lógica avanzada
        var task = new ScheduledTask
        {
            DeviceId = model.SelectedDeviceId,
            ScheduledDateTime = model.ScheduledDateTime.ToUniversalTime(),
            Action = model.SelectedAction,
            SetPointValue = model.SelectedAction == ActionType.ChangeSetPoint
                ? model.SetPointValue
                : null,
            IsRecurring = model.IsRecurring,
            Recurrence = model.IsRecurring
    ? (model.Recurrence.HasValue ? model.Recurrence.Value : RecurrenceType.Daily)
    : null,
            RecurrenceEndDate = model.IsRecurring
                ? (model.HasRecurrenceEnd
                    ? model.RecurrenceEndDate?.ToUniversalTime()
                    : null)
                : model.ScheduledDateTime.ToUniversalTime(),
            Status = ScheduledTaskStatus.Pending
        };

        await _repository.CreateTask(task);

        if (transaccionIniciadaInternamente)
        {
            await _repository.CommitTransaction();
        }
        else
        {
            await _repository.PartialCommit();
        }

        return RedirectToAction("Index");
    }


    [HttpGet]
    public async Task<IActionResult> Index()
    {
        await _repository.BeginTransaction();
        var tasks = await _repository.GetAllTasks(); // usa Include para traer el nombre del dispositivo

        var viewModel = tasks.Select(t => new ScheduledTaskListItemViewModel
        {
            Id = t.Id,
            DeviceName = t.Device?.RoomName ?? $"#{t.DeviceId}",
            Action = t.Action,
            ScheduledDateTime = t.ScheduledDateTime.ToLocalTime(),
            IsRecurring = t.IsRecurring,
            Status = (ScheduledTaskStatus)t.Status,
            SetPointValue = t.SetPointValue
        }).ToList();
        await _repository.CommitTransaction();
        return View(viewModel);
    }
    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _repository.BeginTransaction();
        var task = await _repository.GetTaskById(id);
        if (task != null)
        {
            await _repository.DeleteTask(task);
            await _repository.CommitTransaction();
        }
        else
        {
            await _repository.RollbackTransaction();
        }

        return RedirectToAction("Index");
    }



    private async Task<List<SelectListItem>> LoadDevicesAsync()
    { 
        bool Flag = false;
        if(!_repository.IsInTransaction)
        {
            await _repository.BeginTransaction();
            Flag = true;
        }
        var LoadedDevices = (await ((IDixellRepository)_repository).GetAllDixellsWithoutTemperatures<DixellXR>())
            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.RoomName }).ToList();
        if (Flag)
        {
            await _repository.CommitTransaction();
        }
        else
        {
            await _repository.PartialCommit();
        }
        return LoadedDevices;
    }
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        await _repository.BeginTransaction();
        var task = await _repository.GetTaskById(id);
        if (task == null)
        {
            await _repository.RollbackTransaction();
            return NotFound();
        }

        var viewModel = new ScheduleTaskViewModel
        {
            Id = task.Id,
            SelectedDeviceId = task.DeviceId,
            SelectedAction = task.Action,
            SetPointValue = task.SetPointValue,
            ScheduledDateTime = task.ScheduledDateTime.ToLocalTime(),
            IsRecurring = task.IsRecurring,
            Recurrence = task.Recurrence,
            RecurrenceEndDate = task.RecurrenceEndDate?.ToLocalTime(),
            Status = (ScheduledTaskStatus)task.Status,
            Devices = await LoadDevicesAsync(),
            ActionOptions = LoadActions()
        };

        await _repository.CommitTransaction();
        return View(viewModel);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(ScheduleTaskViewModel model)
    {
        ModelState.Remove(nameof(ScheduleTaskViewModel.Devices));
        ModelState.Remove(nameof(ScheduleTaskViewModel.ActionOptions));

        if (!ModelState.IsValid)
        {
            model.Devices = await LoadDevicesAsync();
            model.ActionOptions = LoadActions();
            return View(model);
        }

        await _repository.BeginTransaction();
        var task = await _repository.GetTaskById(model.Id);
        if (task == null)
        {
            await _repository.RollbackTransaction();
            return NotFound();
        }

        // Asignaciones desde el modelo
        task.DeviceId = model.SelectedDeviceId;
        task.Action = model.SelectedAction;

        // Solo asigna SetPointValue si corresponde
        task.SetPointValue = model.SelectedAction == ActionType.ChangeSetPoint
            ? model.SetPointValue
            : null;

        task.ScheduledDateTime = model.ScheduledDateTime.ToUniversalTime();
        task.IsRecurring = model.IsRecurring;
        task.Recurrence = model.Recurrence;

        // ← lógica solicitada: se asigna automáticamente o queda en null
        task.RecurrenceEndDate = model.IsRecurring
            ? (model.RecurrenceEndDate?.ToUniversalTime() ?? task.ScheduledDateTime.AddDays(30))
            : null;

        task.Status = model.Status;

        await _repository.UpdateTask(task);
        await _repository.CommitTransaction();

        return RedirectToAction("Index");
    }



    private List<SelectListItem> LoadActions() =>
        Enum.GetValues(typeof(ActionType))
            .Cast<ActionType>()
            .Select(a => new SelectListItem { Value = a.ToString(), Text = a.ToString() }).ToList();
}
