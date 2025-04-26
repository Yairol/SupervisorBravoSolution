using SupervisorBravo.Domain.Entities.Dixell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence.Abstracts.Dixells
{
    /// <summary>
    /// Define las operaciones en la BD con los Dixell.
    /// </summary>
    public interface IDixellRepository : IRepository
    {
        /// <summary>
        /// Creacion de un Dixell en la BD.
        /// </summary>
        /// <param name="roomName">Sala que monitorea el Dixell.</param>
        /// <param name="moodbusId">Identificador en el bus moodbus del Dixell.</param>
        /// <returns></returns>
        Task<DixellXR60CX> CreateDixellXR60CX(string roomName, int moodbusId);
        /// <summary>
        /// Obtencion de un Dixell por su id.
        /// </summary>
        /// <typeparam name="T">Dixell generico.</typeparam>
        /// <param name="id">Identificaddor en la base de datos.</param>
        /// <returns></returns>
        Task<T> GetDixellById<T>(Guid id) where T : DixellBase;
        /// <summary>
        /// Obtencion de un Dixell por su id en el bus moodbus.
        /// </summary>
        /// <typeparam name="T">Dixell generico.</typeparam>
        /// <param name="moodbusId">Identificador en el bus moodbus.</param>
        /// <returns></returns>
        Task<T> GetDixellByMoodbusId<T>(int moodbusId) where T : DixellBase;
        /// <summary>
        /// Obtencion de todos los Dixell en la BD.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        Task<List<T>> GetAllDixells<T>() where T : DixellBase;
        /// <summary>
        /// Modificacion de un Dixell en la BD.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dixell"></param>
        /// <returns></returns>
        Task UpdateDixell<T>(T dixell) where T : DixellBase;
        /// <summary>
        /// Eliminacion de un Dixell en la BD.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task  DeleteDixell<T>(Guid id) where T : DixellBase;
    }
}
