namespace SupervisorBravo.Domain.Entities.Dixell
{
    /// <summary>
    /// Modela un Dixell modelo XR60CX.
    /// </summary>
    public class DixellXR60CX : DixellBase
    {
        #region Propiedades

        /// <summary>
        /// Control de encendido y apagado del Dixell.
        /// </summary>
        public bool ControlON_OFF { get; set; }
        /// <summary>
        /// Control de encendido y apagado del deshielo
        /// </summary>
        public bool Thawing { get; set; }

        #endregion

        #region Constructores
        /// <summary>
        /// Crea una instancia de la clase <see cref="DixellXR60CX"/>.
        /// </summary>
        public DixellXR60CX()
        {
            MoodbusId = 0;
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
        public DixellXR60CX(int modbusId, string roomName)
        {
            MoodbusId = modbusId;
            RoomName = roomName;
            ControlON_OFF = true;
            Thawing = false;
            SetPoint = 5;
        }
        #endregion
    }
}
