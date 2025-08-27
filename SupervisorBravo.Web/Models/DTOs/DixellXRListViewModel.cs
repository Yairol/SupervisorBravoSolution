using SupervisorBravo.Domain.Entities.Dixell;

namespace SupervisorBravo.Web.Models.DTOs
{
    public class DixellXRListViewModel
    {
        public List<DixellXR> Devices { get; set; } = new List<DixellXR> { new DixellXR() };
    }
}
