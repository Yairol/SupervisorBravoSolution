using SupervisorBravo.Domain.Entities.Common;
using SupervisorBravo.Domain.Entities.Dixell;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public double TemperatureMeasurement { get; set; }
        /// <summary>
        /// Fecha y hora de la medición de la temperatura.
        /// </summary>
        public DateTime MeasurementTime { get; set; }
        /// <summary>
        /// Identiifcador del Dixell al que pertence la medicion de la temperatura.
        /// </summary>
        public Guid DixellId { get; set; }
        /// <summary>
        /// Referencia al Dixell al que pertenece la medición de la temperatura.
        /// </summary>
        [ForeignKey(nameof(DixellId))]
        public virtual DixellXR60CX Dixell { get; set; }
        #endregion

        #region Constructores
        /// <summary>
        /// Crea una instancia de la clase <see cref="Temperature"/> con valores por defecto.
        /// </summary>
        public Temperature()
        {
            TemperatureMeasurement = 0.0;
            MeasurementTime = DateTime.Now;
        }
        /// <summary>
        /// Crea una instancia de la clase <see cref="Temperature"/> con los valores especificados.
        /// </summary>
        /// <param name="temperatureMeasurement">Valor de la empeatura medida por el Dixell.</param>
        /// <param name="measurementTime">Fecha y hora de la medición de la temperatura.</param>
        public Temperature(double temperatureMeasurement, DateTime measurementTime)
        {
            TemperatureMeasurement = temperatureMeasurement;
            MeasurementTime = measurementTime;
        }
        #endregion
    }
}
