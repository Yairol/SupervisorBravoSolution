namespace SupervisorBravo.Domain.Entities.Dixell
{
    /// <summary>
    /// Clase que modela un dixell XT111C
    /// </summary>
    public class DixellXT : DixellBase
    {

        public bool ElectroValve { get; set; }
        /// <summary>
        /// Constructor por defecto.
        /// </summary>
        public DixellXT()
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
        public DixellXT(int modbusId, string roomNAme)
        {
            MoodbusId = modbusId;
            RoomName = roomNAme;
            SetPointWrite = false;
            ControlON_OFFWrite = false;
        }
    }
}
