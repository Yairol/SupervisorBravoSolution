using Microsoft.EntityFrameworkCore;

namespace SupervisorBravo.Persistence.Repository
{
    public partial class AplicationRepository : IAnalogVariableRepository
    {
        public async Task<List<PLCAnalogVariable>> GetAllAnalogVariablesAsync()
        {
            var ctx = EnsureContext();
            return await ctx.Set<PLCAnalogVariable>().ToListAsync();
        }

        public async Task<PLCAnalogVariable?> GetAnalogVariableByIdAsync(Guid id)
        {
            var ctx = EnsureContext();
            return await ctx.Set<PLCAnalogVariable>().FindAsync(id);
        }

        public async Task AddAnalogVariableAsync(PLCAnalogVariable variable)
        {
            var ctx = EnsureContext();
            await ctx.Set<PLCAnalogVariable>().AddAsync(variable);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateAnalogVariableAsync(PLCAnalogVariable variable)
        {
            var ctx = EnsureContext();
            ctx.Set<PLCAnalogVariable>().Update(variable);
            await ctx.SaveChangesAsync();
        }

        public async Task DeleteAnalogVariableAsync(Guid id)
        {
            var ctx = EnsureContext();
            var variable = await ctx.Set<PLCAnalogVariable>().FindAsync(id);
            if (variable is not null)
            {
                ctx.Set<PLCAnalogVariable>().Remove(variable);
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdAsync(Guid deviceId)
        {
            var ctx = EnsureContext();
            return await ctx.Set<PLCAnalogVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .ToListAsync();
        }

        public async Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdWithMeasurementsAsync(Guid deviceId)
        {
            var ctx = EnsureContext();
            return await ctx.Set<PLCAnalogVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .Include(v => v.Measurements)
                .ToListAsync();
        }

        public async Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdWithMeasurementsAsync(
            Guid deviceId, DateTime fromUtc, DateTime toUtc)
        {
            // Normaliza a UTC
            if (fromUtc.Kind != DateTimeKind.Utc) fromUtc = fromUtc.ToUniversalTime();
            if (toUtc.Kind != DateTimeKind.Utc) toUtc = toUtc.ToUniversalTime();

            // Garantiza rango correcto
            if (fromUtc > toUtc) (fromUtc, toUtc) = (toUtc, fromUtc);

            var ctx = EnsureContext();
            return await ctx.Set<PLCAnalogVariable>()
                .Where(v => v.PLCDeviceId == deviceId)
                .Include(v => v.Measurements
                    .Where(m => m.MeasurementTime >= fromUtc && m.MeasurementTime <= toUtc)
                    .OrderBy(m => m.MeasurementTime))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<PLCAnalogVariable>> GetAnalogVariableByDeviceIdWithLastMeasurementAsync(Guid deviceId)
        {
            var ctx = EnsureContext();
            return await ctx.Set<PLCAnalogVariable>()
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
