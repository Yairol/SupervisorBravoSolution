using System.ComponentModel.DataAnnotations;

namespace SupervisorBravo.Web.Models.Variable
{
    public class VariableCreateViewModel
    {
        public Guid PlcId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección Modbus es obligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "La dirección debe ser un número positivo.")]
        public int Address { get; set; }

        public bool IsWritable { get; set; }

        [Required(ErrorMessage = "Debes seleccionar el tipo de variable.")]
        public string Type { get; set; } = "Digital"; // "Digital" o "Analogica"

        [Range(0, 7, ErrorMessage = "El BitIndex debe estar entre 0 y 7.")]
        public int? BitIndex { get; set; } // Solo para digitales
    }

}
