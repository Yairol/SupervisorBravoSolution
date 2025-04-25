using SupervisorBravo.Domain.Entities.Common;
using SupervisorBravo.Domain.Entities.Temperatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string RoomName { get; set; }
        /// <summary>
        /// Identificador en el bus Moodbus.
        /// </summary>
        public int MoodbusId { get; set; }
        /// <summary>
        /// Valor del SetPoint.
        /// </summary>
        public double SetPoint { get; set; }
        /// <summary>
        /// Lecturas de las temperaturas medididas por el Dixell.
        /// </summary>
        public virtual ICollection<Temperature> temperatures { get; set; } = new List<Temperature>();

    }
}
