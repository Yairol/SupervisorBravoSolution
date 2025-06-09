using SupervisorBravo.Domain.Entities.Common;

namespace SupervisorBravo.Domain.Entities.System
{
    /// <summary>
    /// Modela una alarma general.
    /// </summary>
    public class Alarm : Entity
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
        #endregion
        #region Constructores
        /// <summary>
        /// Constructor de la alarma.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public Alarm(string name, string description)
        {
            Name = name;
            Description = description;
            AlarmDate = DateTime.UtcNow;

        }
        /// <summary>
        /// Constructor por defecto.
        /// </summary>
        public Alarm()
        {
            Name = string.Empty;
            Description = string.Empty;
        }
        #endregion
    }
}
