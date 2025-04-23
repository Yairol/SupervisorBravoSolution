using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Domain.Entities.Common
{
    /// <summary>
    /// Modela una entidad genérica.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Identificador para soporte de datos.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }
}
