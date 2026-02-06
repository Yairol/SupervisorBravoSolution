namespace SupervisorBravo.Web.ViewModels
{
    public class ResumenViewModel
    {
        public List<DeviceCheckViewModel> Devices { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class DeviceCheckViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}