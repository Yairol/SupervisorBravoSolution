using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Dixells;

namespace SupervisorBravo.Persistence.Repository
{

    public partial class AplicationRepository : IDixellRepository
    {

        public async Task<DixellXT111C> CreateDixellXT111C(string roomName, int moodbusId)
        {
            DixellXT111C dixellXT111C = new DixellXT111C(moodbusId, roomName);
            await _context.AddAsync(dixellXT111C);
            return dixellXT111C;
        }
        public async Task<DixellXR60CX> CreateDixellXR60CX(string roomName, int moodbusId)
        {
            DixellXR60CX dixellXR60CX = new DixellXR60CX(moodbusId, roomName);
            await _context.AddAsync(dixellXR60CX);
            return dixellXR60CX;
        }

        public async Task DeleteDixell<T>(Guid id) where T : DixellBase
        {
            var dixell = await _context.Set<T>().FindAsync(id);
            if (dixell != null)
            {
                _context.Set<T>().Remove(dixell);
            }
        }

        public async Task<List<T>> GetAllDixells<T>() where T : DixellBase
        {
            return await _context.Set<T>().Include(d => d.temperatures).ToListAsync();
        }

        public async Task<T> GetDixellById<T>(Guid id) where T : DixellBase
        {
            return await _context.Set<T>().FindAsync(id);

        }

        public async Task<T> GetDixellByMoodbusId<T>(int moodbusId) where T : DixellBase
        {
            return await _context.Set<T>().FirstOrDefaultAsync(t => t.MoodbusId == moodbusId);
        }

        public Task UpdateDixell<T>(T dixell) where T : DixellBase
        {
            _context.Set<T>().Update(dixell);
            return Task.CompletedTask;
        }

        public async Task<T?> GetDixellByRoomName<T>(string roomName) where T : DixellBase
        {
            return await _context.Set<T>().FirstOrDefaultAsync(d => d.RoomName == roomName);
        }
    }
}
