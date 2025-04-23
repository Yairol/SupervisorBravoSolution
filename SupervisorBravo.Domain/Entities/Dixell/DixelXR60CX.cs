using SupervisorBravo.Domain.Entities.Common;
using SupervisorBravo.Domain.Entities.Temperatures;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Domain.Entities.Dixell
{
    /// <summary>
    /// Modela un Dixell modelo XR60CX.
    /// </summary>
    public class DixellXR60CX : Entity
    {
        #region Propiedades
        /// <summary>
        /// Identificador del Dixell en el bus modbus.
        /// </summary>
        public int ModbusId { get; set; }
        /// <summary>
        /// Nombre de la sala donde se encuentra el Dixell.
        /// </summary>
        public string RoomName { get; set; }
        /// <summary>
        /// Control de encendido y apagado del Dixell.
        /// </summary>
        public bool ControlON_OFF { get; set; }
        /// <summary>
        /// Control de encendido y apagado del deshielo
        /// </summary>
        public bool Thawing { get; set; }
        /// <summary>
        /// Valor del setpoint del Dixell.
        /// </summary>
        public double SetPoint { get; set; }
        /// <summary>
        /// Colección de temperaturas medidas por el Dixell.
        /// </summary>
        public virtual ICollection<Temperature> temperatures { get ; set; } = new List<Temperature>();
        #endregion

        #region Constructores
        /// <summary>
        /// Crea una instancia de la clase <see cref="DixellXR60CX"/>.
        /// </summary>
        public DixellXR60CX()
        {
            ModbusId = 0;
            RoomName = string.Empty;
            ControlON_OFF = false;
            Thawing = false;
            SetPoint = 0.0;
        }
        /// <summary>
        /// Crea una instancia de la clase <see cref="DixellXR60CX"/> con los valores especificados.
        /// </summary>
        /// <param name="modbusId">Identificador del Dixel en el bus Modbus.</param>
        /// <param name="roomName">Nombre de la sala que se encuentra el Dixell.</param>
        /// <param name="controlON_OFF">Estado del control ON/OFF del Dixell.</param>
        /// <param name="thawing">Estado del deshielo del Dixell.</param>
        /// <param name="setPoint">Valor del SetPoint del Dixell.</param>
        public DixellXR60CX(int modbusId, string roomName, double setPoint)
        {
            ModbusId = modbusId;
            RoomName = roomName;
            ControlON_OFF = true;
            Thawing = false;
            SetPoint = setPoint;
        }
        #endregion
    }
}
