using SupervisorBravo.Domain.Entities.Dixell;

namespace SupervisorBravo.Web.Models.DTOs
{
    public class DixellXTListViewModel
    {
        public List<DixellXT> Devices { get; set; } = new List<DixellXT> { new DixellXT() };
    }
}
