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
        public async Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdWithMeasurementsAsync(
            Guid deviceId, DateTime fromUtc, DateTime toUtc)
        {
            // Normaliza a UTC para que el filtro sea consistente con PostgreSQL
            if (fromUtc.Kind != DateTimeKind.Utc) fromUtc = fromUtc.ToUniversalTime();
            if (toUtc.Kind != DateTimeKind.Utc) toUtc = toUtc.ToUniversalTime();

            // Garantiza rango correcto
            if (fromUtc > toUtc) (fromUtc, toUtc) = (toUtc, fromUtc);

            return await _context.Set<PLCDigitalVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .Include(v => v.Measurements
                    .Where(m => m.MeasurementTime >= fromUtc && m.MeasurementTime <= toUtc)
                    .OrderBy(m => m.MeasurementTime))
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdWithLastMeasurementAsync(Guid deviceId)
        {
            return await _context.Set<PLCDigitalVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .Select(v => new PLCDigitalVariable
                {
                    Id = v.Id,
                    Name = v.Name,
                    Address = v.Address,
                    BitIndex = v.BitIndex,
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

