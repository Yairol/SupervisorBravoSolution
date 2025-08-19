using Microsoft.EntityFrameworkCore;
using System;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IPLCDeviceRepository
    {
        public async Task<List<PLCDevice>> GetAllPLCDeviceAsync(bool includeVariables = false)
        {
            var ctx = EnsureContext();
            var query = ctx.Set<PLCDevice>().AsQueryable();

            if (includeVariables)
                query = query.Include(d => d.Variables);

            return await query.ToListAsync();
        }

        public async Task<PLCDevice?> GetPLCDeviceByIdAsync(Guid id, bool includeVariables = false)
        {
            var ctx = EnsureContext();
            var query = ctx.Set<PLCDevice>().AsQueryable();

            if (includeVariables)
                query = query.Include(d => d.Variables);

            return await query.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<PLCDevice>> GetPLCDeviceByNameAsync(string namePart)
        {
            var ctx = EnsureContext();
            return await ctx.PLCDevices
                .Where(d => d.Name.Contains(namePart))
                .ToListAsync();
        }

        public async Task AddPLCDeviceAsync(PLCDevice device)
        {
            var ctx = EnsureContext();
            await ctx.Set<PLCDevice>().AddAsync(device);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdatePLCDeviceAsync(PLCDevice device)
        {
            var ctx = EnsureContext();
            ctx.PLCDevices.Update(device);
            await ctx.SaveChangesAsync();
        }

        public async Task DeletePLCDeviceAsync(Guid id)
        {
            var ctx = EnsureContext();
            var device = await ctx.Set<PLCDevice>().FindAsync(id);
            if (device is not null)
            {
                ctx.PLCDevices.Remove(device);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
