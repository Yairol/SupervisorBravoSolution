using System.ComponentModel.DataAnnotations;

namespace SupervisorBravo.Web.Models.PLC
{
    public class PLCDeviceViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección Modbus es obligatoria.")]
        [Range(1, 247, ErrorMessage = "La dirección Modbus debe estar entre 1 y 247.")]
        public int ModbusAddress { get; set; }

        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "La dirección IP no es válida.")]
        public string? IPAddress { get; set; } = string.Empty;
    }

}
