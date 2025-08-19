using SupervisorBravo.Domain.Entities.Temperatures;

namespace SupervisorBravo.Web.Models.DTOs
{
    public class TemperatureFilterViewModel
    {
        public string DixellName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<Temperature> Temperatures { get; set; } = new();
    }

}
