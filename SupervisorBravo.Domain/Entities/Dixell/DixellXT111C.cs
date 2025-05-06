namespace SupervisorBravo.Domain.Entities.Dixell
{
    /// <summary>
    /// Clase que modela un dixell XT111C
    /// </summary>
    public class DixellXT111C : DixellBase
    {
        /// <summary>
        /// Control On/Off activo
        /// </summary>
        public bool ControlON_OFF { get; set; }

        /// <summary>
        /// Constructor por defecto.
        /// </summary>
        public DixellXT111C()
        {
            MoodbusId = 0;
            RoomName = string.Empty;
            ControlON_OFF = false;
            SetPoint = 0.0;
        }
        /// <summary>
        /// Constructor para crear un dixell XT111C.
        /// </summary>
        /// <param name="modbusId">Identificador en el bus modbus.</param>
        /// <param name="roomNAme">Nombre de la habitacion que monitorea el dixell.</param>
        public DixellXT111C(int modbusId, string roomNAme)
        {
            MoodbusId = modbusId;
            RoomName = roomNAme;
        }
    }
}
