using SupervisorBravo.Domain.Entities.Common;

namespace SupervisorBravo.Domain.Entities.System
{
    public class DeviceAlarm : Entity
    {
        #region Propiedades
        /// <summary>
        /// Nombre de la alarma.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Descripcion de la alarma.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Fecha en que se genero la alarma.
        /// </summary>
        public DateTime AlarmDate { get; set; }
        /// <summary>
        /// Nombre del dispositivo que genero la alarma.
        /// </summary>
        public string DeviceName { get; set; }
        #endregion
        #region Constructores
        /// <summary>
        /// Constructor de la alarma.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public DeviceAlarm(string name, string description, string deviceName)
        {
            Name = name;
            Description = description;
            DeviceName = deviceName;
            AlarmDate = DateTime.UtcNow;

        }
        /// <summary>
        /// Constructor por defecto.
        /// </summary>
        public DeviceAlarm()
        {
            Name = string.Empty;
            Description = string.Empty;
            DeviceName = string.Empty;
        }
        #endregion

    }
}
