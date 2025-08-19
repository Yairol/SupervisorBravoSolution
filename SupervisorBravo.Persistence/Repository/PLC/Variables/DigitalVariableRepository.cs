using Microsoft.EntityFrameworkCore;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IDigitalVariableRepository
    {
        public async Task<List<PLCDigitalVariable>> GetAllDigitalVariablesAsync()
        {
            var ctx = EnsureContext();
            return await ctx.Set<PLCDigitalVariable>().ToListAsync();
        }

        public async Task<PLCDigitalVariable?> GetDigitalVariableByIdAsync(Guid id)
        {
            var ctx = EnsureContext();
            return await ctx.Set<PLCDigitalVariable>().FindAsync(id);
        }

        public async Task AddDigitalVariableAsync(PLCDigitalVariable variable)
        {
            var ctx = EnsureContext();
            await ctx.Set<PLCDigitalVariable>().AddAsync(variable);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateDigitalVariableAsync(PLCDigitalVariable variable)
        {
            var ctx = EnsureContext();
            ctx.Set<PLCDigitalVariable>().Update(variable);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteDigitalVariableAsync(Guid id)
        {
            var ctx = EnsureContext();
            var variable = await ctx.Set<PLCDigitalVariable>().FindAsync(id);
            if (variable is not null)
            {
                ctx.Set<PLCDigitalVariable>().Remove(variable);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdAsync(Guid deviceId)
        {
            var ctx = EnsureContext();
            return await ctx.Set<PLCDigitalVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .ToListAsync();
        }

        public async Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdWithMeasurementsAsync(Guid deviceId)
        {
            var ctx = EnsureContext();
            return await ctx.Set<PLCDigitalVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .Include(v => v.Measurements)
                .ToListAsync();
        }

        public async Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdWithMeasurementsAsync(
            Guid deviceId, DateTime fromUtc, DateTime toUtc)
        {
            // Normaliza a UTC
            if (fromUtc.Kind != DateTimeKind.Utc) fromUtc = fromUtc.ToUniversalTime();
            if (toUtc.Kind != DateTimeKind.Utc) toUtc = toUtc.ToUniversalTime();

            // Garantiza rango correcto
            if (fromUtc > toUtc) (fromUtc, toUtc) = (toUtc, fromUtc);

            var ctx = EnsureContext();
            return await ctx.Set<PLCDigitalVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .Include(v => v.Measurements
                    .Where(m => m.MeasurementTime >= fromUtc && m.MeasurementTime <= toUtc)
                    .OrderBy(m => m.MeasurementTime))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PLCDigitalVariable>> GetDigitalVariableByDeviceIdWithLastMeasurementAsync(Guid deviceId)
        {
            var ctx = EnsureContext();
            return await ctx.Set<PLCDigitalVariable>()
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
