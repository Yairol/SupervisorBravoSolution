using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SupervisorBravo.Persistence.Abstracts;

namespace SupervisorBravo.Persistence.Repository
{
    /// <summary>
    /// Repositorio asociado a las entidades de la aplicacion.
    /// </summary>
    public partial class AplicationRepository : IRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        /// <summary>
        /// Contexto mediante el cual se establece la conexion a la BD.
        /// </summary>
        private ApplicationDbContext? _context;
        /// <summary>
        /// Indicador de transaccion.
        /// </summary>
        private IDbContextTransaction? _transaction;

        /// <summary>
        /// Inicializa un objeto <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="context"></param>
        public AplicationRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public bool IsInTransaction => _transaction != null;

        /// <summary>
        /// Inicia una transacción.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task
            BeginTransaction()
        {
            if (IsInTransaction)
                throw new InvalidOperationException("Cannot begin a new transaction before closing the current one.");
            _context = await _contextFactory.CreateDbContextAsync();

            _transaction = await _context.Database.BeginTransactionAsync();
            await _context.Database.CanConnectAsync();
            //await _context.Database.MigrateAsync();
        }
        /// <summary>
        /// Guarda los cambios de la transacción actual y la cierra.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task CommitTransaction()
        {
            if (!IsInTransaction)
                throw new InvalidOperationException("There is no open transaction to commit.");

            await _context!.SaveChangesAsync();
            await _transaction!.CommitAsync();
            await _transaction.DisposeAsync();

            await _context.DisposeAsync();
            _transaction = null;
            _context = null;
        }
        /// <summary>
        /// Guarda los cambios de la transacción actual sin cerrarla.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task PartialCommit()
        {
            if (_context is null)
                throw new InvalidOperationException("There is no open transaction.");

            await _context.SaveChangesAsync();
        }
        /// <summary>
        /// Elimina la transacción actual sin guardar los cambios en BD.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task RollbackTransaction()
        {
            if (!IsInTransaction)
                throw new InvalidOperationException("There is no open transaction to rollback.");

            await _transaction!.RollbackAsync();
            await _transaction.DisposeAsync();
            await _context!.DisposeAsync();

            _transaction = null;
            _context = null;
        }
    }
}
