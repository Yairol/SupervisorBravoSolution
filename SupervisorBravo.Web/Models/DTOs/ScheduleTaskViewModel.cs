using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using SupervisorBravo.Domain.Entities.Schedule;
using System.ComponentModel.DataAnnotations;

public class ScheduleTaskViewModel
{
    public Guid Id { get; set; }
    public Guid SelectedDeviceId { get; set; }

    [Display(Name = "Fecha de ejecución")]
    public DateTime ScheduledDateTime { get; set; }

    [Display(Name = "Tipo de acción")]
    public ActionType SelectedAction { get; set; }

    [Display(Name = "Valor de SetPoint (si aplica)")]
    public double? SetPointValue { get; set; }

    [Display(Name = "¿Es una tarea recurrente?")]
    public bool IsRecurring { get; set; }

    [Display(Name = "Tipo de recurrencia")]
    public RecurrenceType? Recurrence { get; set; }

    [Display(Name = "Fecha de fin de recurrencia")]
    public DateTime? RecurrenceEndDate { get; set; }

    [BindNever]
    public List<SelectListItem> Devices { get; set; }
    [BindNever]
    public List<SelectListItem> ActionOptions { get; set; }
    public ScheduledTaskStatus Status { get; set; }
    public bool HasRecurrenceEnd { get; set; } // para controlar si mostrar campo EndDate


}
