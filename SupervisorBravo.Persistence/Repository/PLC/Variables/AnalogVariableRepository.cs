using Microsoft.EntityFrameworkCore;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IAnalogVariableRepository
    {
        public async Task<List<PLCAnalogVariable>> GetAllAnalogVariablesAsync()
        {
            return await _context.Set<PLCAnalogVariable>().ToListAsync();
        }

        public async Task<PLCAnalogVariable?> GetAnalogVariableByIdAsync(Guid id)
        {
            return await _context.Set<PLCAnalogVariable>().FindAsync(id);
        }

        public async Task AddAnalogVariableAsync(PLCAnalogVariable variable)
        {
            await _context.Set<PLCAnalogVariable>().AddAsync(variable);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAnalogVariableAsync(PLCAnalogVariable variable)
        {
            _context.Set<PLCAnalogVariable>().Update(variable);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnalogVariableAsync(Guid id)
        {
            var variable = await _context.Set<PLCAnalogVariable>().FindAsync(id);
            if (variable is not null)
            {
                _context.Set<PLCAnalogVariable>().Remove(variable);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdAsync(Guid deviceId)
        {
            return await _context.Set<PLCAnalogVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .ToListAsync();
        }
    }
}

