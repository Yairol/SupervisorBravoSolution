using Microsoft.EntityFrameworkCore;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IDigitalVariableRepository
    {
        public async Task<List<PLCDigitalVariable>> GetAllDigitalVariablesAsync()
        {
            return await _context.Set<PLCDigitalVariable>().ToListAsync();
        }

        public async Task<PLCDigitalVariable?> GetDigitalVariableByIdAsync(Guid id)
        {
            return await _context.Set<PLCDigitalVariable>().FindAsync(id);
        }

        public async Task AddDigitalVariableAsync(PLCDigitalVariable variable)
        {
            await _context.Set<PLCDigitalVariable>().AddAsync(variable);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDigitalVariableAsync(PLCDigitalVariable variable)
        {
            _context.Set<PLCDigitalVariable>().Update(variable);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDigitalVariableAsync(Guid id)
        {
            var variable = await _context.Set<PLCDigitalVariable>().FindAsync(id);
            if (variable is not null)
            {
                _context.Set<PLCDigitalVariable>().Remove(variable);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdAsync(Guid deviceId)
        {
            return await _context.Set<PLCDigitalVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .ToListAsync();
        }
        public async Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdWithMeasurementsAsync(Guid deviceId)
        {
            return await _context.Set<PLCDigitalVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .Include(v => v.Measurements)
                .ToListAsync();
        }
    }
}

