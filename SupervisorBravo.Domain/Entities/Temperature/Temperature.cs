using SupervisorBravo.Domain.Entities.Common;
using SupervisorBravo.Domain.Entities.Dixell;
using System.ComponentModel.DataAnnotations.Schema;

namespace SupervisorBravo.Domain.Entities.Temperatures
{
    /// <summary>
    /// Modela el valor de la temperatura mediad por un Dixell.
    /// </summary>
    public class Temperature : Entity
    {
        #region Propiedades
        /// <summary>
        /// Valor de la temperatura medida por el Dixell.
        /// </summary>
        public double TemperatureMeasurement { get; private set; }
        /// <summary>
        /// Fecha y hora de la medición de la temperatura.
        /// </summary>
        public DateTime MeasurementTime { get; private set; }
        /// <summary>
        /// Identiifcador del Dixell al que pertence la medicion de la temperatura.
        /// </summary>
        public Guid DixellId { get; set; }
        /// <summary>
        /// Referencia al Dixell al que pertenece la medición de la temperatura.
        /// </summary>
        [ForeignKey(nameof(DixellId))]
        public virtual DixellBase Dixell { get; set; }
        #endregion

        #region Constructores
        /// <summary>
        /// Crea una instancia de la clase <see cref="Temperature"/> con valores por defecto.
        /// </summary>
        public Temperature()
        {

        }
        /// <summary>
        /// Crea una instancia de la clase <see cref="Temperature"/> con los valores especificados.
        /// </summary>
        /// <param name="temperatureMeasurement">Valor de la empeatura medida por el Dixell.</param>
        /// <param name="measurementTime">Fecha y hora de la medición de la temperatura.</param>
        /// <param name="dixellId">Identificador del Dixell que hizo la lectura.</param>
        public Temperature(double temperatureMeasurement, DateTime measurementTime, Guid dixellId)
        {
            TemperatureMeasurement = temperatureMeasurement;
            MeasurementTime = measurementTime;
            DixellId = dixellId;
        }
        #endregion
    }
}
