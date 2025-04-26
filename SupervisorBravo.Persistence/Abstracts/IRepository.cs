using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence.Abstracts
{
    /// <summary>
    /// Define las propiedades y métodos de un repositorio.
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Indica si el repositorio se encuentra en una transacción.
        /// </summary>
        bool IsInTransaction { get; }

        /// <summary>
        /// Inicia una transacción.
        /// </summary>
        Task BeginTransaction();

        /// <summary>
        /// Guarda los cambios de la transacción actual y la cierra.
        /// </summary>
        Task CommitTransaction();

        /// <summary>
        /// Elimina la transacción actual sin guardar los cambios en BD.
        /// </summary>
        Task RollbackTransaction();

        /// <summary>
        /// Guarda los cambios de la transacción actual sin cerrarla.
        /// </summary>
        Task PartialCommit();

    }
}
