using Microsoft.EntityFrameworkCore;
using System;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IPLCDeviceRepository
    {
        public async Task<List<PLCDevice>> GetAllPLCDeviceAsync(bool includeVariables = false)
        {
            var query = _context.Set<PLCDevice>().AsQueryable();

            if (includeVariables)
                query = query.Include(d => d.Variables);

            return await query.ToListAsync();
        }

        public async Task<PLCDevice?> GetPLCDeviceByIdAsync(Guid id, bool includeVariables = false)
        {
            var query = _context.Set<PLCDevice>().AsQueryable();

            if (includeVariables)
                query = query.Include(d => d.Variables);

            return await query.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<PLCDevice>> GetPLCDeviceByNameAsync(string namePart)
        {
            return await _context.PLCDevices
                .Where(d => d.Name.Contains(namePart))
                .ToListAsync();
        }

        public async Task AddPLCDeviceAsync(PLCDevice device)
        {
            await _context.Set<PLCDevice>().AddAsync(device);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePLCDeviceAsync(PLCDevice device)
        {
            _context.PLCDevices.Update(device);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePLCDeviceAsync(Guid id)
        {
            var device = await _context.Set<PLCDevice>().FindAsync(id);
            if (device is not null)
            {
                _context.PLCDevices.Remove(device);
                await _context.SaveChangesAsync();
            }
        }
    }

}
