using Microsoft.EntityFrameworkCore;
using SupervisorBravo.Domain.Entities.Dixell;
using SupervisorBravo.Persistence.Abstracts.Dixells;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IDixellRepository
    {
        public async Task<DixellXT> CreateDixellXT111C(string roomName, int moodbusId)
        {
            var ctx = EnsureContext();
            DixellXT dixellXT111C = new DixellXT(moodbusId, roomName);
            await ctx.AddAsync(dixellXT111C);
            return dixellXT111C;
        }

        public async Task<DixellXR> CreateDixellXR60CX(string roomName, int moodbusId, string modelName)
        {
            var ctx = EnsureContext();
            DixellXR dixellXR60CX = new DixellXR(moodbusId, roomName)
            {
                modelName = modelName
            };
            await ctx.AddAsync(dixellXR60CX);
            return dixellXR60CX;
        }

        public async Task DeleteDixell<T>(Guid id) where T : DixellBase
        {
            var ctx = EnsureContext();
            var dixell = await ctx.Set<T>().FindAsync(id);
            if (dixell is not null)
            {
                ctx.Set<T>().Remove(dixell);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<List<T>> GetAllDixells<T>() where T : DixellBase
        {
            var ctx = EnsureContext();
            return await ctx.Set<T>()
                .Include(d => d.temperatures)
                .ToListAsync();
        }
        public async Task<T> GetDixellById<T>(Guid id) where T : DixellBase
        {
            var ctx = EnsureContext();
            return await ctx.Set<T>().FindAsync(id)
                   ?? throw new KeyNotFoundException($"No se encontró {typeof(T).Name} con Id {id}");
        }

        public async Task<T> GetDixellByMoodbusId<T>(int moodbusId) where T : DixellBase
        {
            var ctx = EnsureContext();
            return await ctx.Set<T>().FirstOrDefaultAsync(t => t.MoodbusId == moodbusId)
                   ?? throw new KeyNotFoundException($"No se encontró {typeof(T).Name} con MoodbusId {moodbusId}");
        }

        public Task UpdateDixell<T>(T dixell) where T : DixellBase
        {
            var ctx = EnsureContext();
            ctx.Set<T>().Update(dixell);
            return Task.CompletedTask;
        }

        public async Task<T?> GetDixellByRoomName<T>(string roomName) where T : DixellBase
        {
            var ctx = EnsureContext();
            return await ctx.Set<T>().FirstOrDefaultAsync(d => d.RoomName == roomName);
        }

        public async Task<List<T>> GetAllDixellsWithoutTemperatures<T>() where T : DixellBase
        {
            var ctx = EnsureContext();
            return await ctx.Set<T>().ToListAsync();
        }
        /*public async Task<List<T>> GetAllDixellsWithLastTemperature<T>(Guid deviceId) where T : DixellBase
        {

        }*/
    }
}
