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
        public async Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdWithMeasurementsAsync(Guid deviceId)
        {
            return await _context.Set<PLCAnalogVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .Include(v => v.Measurements)
                .ToListAsync();
        }
        public async Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdWithMeasurementsAsync(
            Guid deviceId, DateTime fromUtc, DateTime toUtc)
        {
            // Normaliza a UTC para que el filtro sea consistente con PostgreSQL
            if (fromUtc.Kind != DateTimeKind.Utc) fromUtc = fromUtc.ToUniversalTime();
            if (toUtc.Kind != DateTimeKind.Utc) toUtc = toUtc.ToUniversalTime();

            // Garantiza rango correcto
            if (fromUtc > toUtc) (fromUtc, toUtc) = (toUtc, fromUtc);

            return await _context.Set<PLCAnalogVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .Include(v => v.Measurements
                    .Where(m => m.MeasurementTime >= fromUtc && m.MeasurementTime <= toUtc)
                    .OrderBy(m => m.MeasurementTime))
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdWithLastMeasurementAsync(Guid deviceId)
        {
            return await _context.Set<PLCAnalogVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .Select(v => new PLCAnalogVariable
                {
                    Id = v.Id,
                    Name = v.Name,
                    Address = v.Address,
                    Type = v.Type,
                    IsWritable = v.IsWritable,
                    PLCDeviceId = v.PLCDeviceId,
                    Measurements = v.Measurements
                        .OrderByDescending(m => m.MeasurementTime)
                        .Take(1)
                        .ToList()
                })
                .AsNoTracking()
                .ToListAsync();
        }




    }
}

