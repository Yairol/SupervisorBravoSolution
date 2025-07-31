using System.ComponentModel.DataAnnotations;

namespace SupervisorBravo.Web.Models.PLC
{
    public class PLCDeviceEditViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección Modbus es obligatoria.")]
        [Range(1, 255, ErrorMessage = "La dirección Modbus debe estar entre 1 y 255.")]
        public int ModbusId { get; set; }

        // IP opcional: solo se valida si se ingresa algo
        [RegularExpression(@"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$", ErrorMessage = "La dirección IP no es válida.")]
        public string? IpAddress { get; set; }
    }


}
