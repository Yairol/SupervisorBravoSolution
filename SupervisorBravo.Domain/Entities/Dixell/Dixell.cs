using SupervisorBravo.Domain.Entities.Common;
using SupervisorBravo.Domain.Entities.Temperatures;
using System.ComponentModel.DataAnnotations;

namespace SupervisorBravo.Domain.Entities.Dixell
{
    /// <summary>
    /// Clase que modela un Dixell generico.
    /// </summary>
    public abstract class DixellBase : Entity
    {
        /// <summary>
        /// Nombre de la sala que monitorea.
        /// </summary>
        [Required(ErrorMessage = "El campo nombre de la sala es obligatorio.")]
        public string RoomName { get; set; } = string.Empty;
        /// <summary>
        /// Control On/Off activo
        /// </summary>
        public bool ControlON_OFF { get; set; }
        /// <summary>
        /// Identificador si el control On/Off es de escritura.
        /// </summary>
        public bool ControlON_OFFWrite { get; set; }
        /// <summary>
        /// Identificador en el bus Moodbus.
        /// </summary>
        [Required(ErrorMessage = "El campo identificador modbus es obligatorio.")]
        public int MoodbusId { get; set; }
        /// <summary>
        /// Valor del SetPoint.
        /// </summary>
        public double SetPoint { get; set; }
        /// <summary>
        /// Identificador si el SetPoint es de escritura.
        /// </summary>
        public bool SetPointWrite { get; set; }
        /// <summary>
        /// Lecturas de las temperaturas medididas por el Dixell.
        /// </summary>
        public virtual ICollection<Temperature> temperatures { get; set; } = null!;
        /// <summary>
        /// Indicador de alarmas habilitadas.
        /// </summary>
        public bool AlarmEnable { get; set; }

    }
}
