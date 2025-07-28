public class ScheduledTaskListFilterViewModel
{
    public List<ScheduledTaskListItemViewModel> Items { get; set; } = new();

    public string? SelectedDeviceName { get; set; }
    public string? SelectedAction { get; set; }
    public string? SelectedStatus { get; set; }

    public List<string> AvailableDeviceNames { get; set; } = new();
    public List<string> AvailableActions { get; set; } = new();
    public List<string> AvailableStatuses { get; set; } = new();

    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}
