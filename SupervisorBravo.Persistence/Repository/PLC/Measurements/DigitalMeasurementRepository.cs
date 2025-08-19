using Microsoft.EntityFrameworkCore;

namespace SupervisorBravo.Persistence.Repository;

public partial class AplicationRepository : IDigitalMeasurementRepository
{
    public async Task<List<DigitalMeasurement>> GetAllDigitalMeasurementsAsync()
    {
        var ctx = EnsureContext();
        return await ctx.Set<DigitalMeasurement>().ToListAsync();
    }

    public async Task<DigitalMeasurement?> GetDigitalMeasurementByIdAsync(Guid id)
    {
        var ctx = EnsureContext();
        return await ctx.Set<DigitalMeasurement>().FindAsync(id);
    }

    public async Task AddDigitalMeasurementAsync(DigitalMeasurement measurement)
    {
        var ctx = EnsureContext();
        await ctx.Set<DigitalMeasurement>().AddAsync(measurement);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateDigitalMeasurementAsync(DigitalMeasurement measurement)
    {
        var ctx = EnsureContext();
        ctx.Set<DigitalMeasurement>().Update(measurement);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteDigitalMeasurementAsync(Guid id)
    {
        var ctx = EnsureContext();
        var entry = await ctx.Set<DigitalMeasurement>().FindAsync(id);
        if (entry is not null)
        {
            ctx.Set<DigitalMeasurement>().Remove(entry);
            await ctx.SaveChangesAsync();
        }
    }

    public async Task<List<DigitalMeasurement>> GetDigitalMeasurementsByVariableIdAsync(Guid digitalVariableId)
    {
        var ctx = EnsureContext();
        return await ctx.Set<DigitalMeasurement>()
            .Where(m => m.PLCDigitalVariableId == digitalVariableId)
            .ToListAsync();
    }

    public async Task<List<DigitalMeasurement>> GetDigitalMeasurementsByTimeRangeAsync(Guid digitalVariableId, DateTime from, DateTime to)
    {
        if (from > to)
        {
            (from, to) = (to, from);
        }

        var ctx = EnsureContext();
        return await ctx.Set<DigitalMeasurement>()
            .Where(m => m.PLCDigitalVariableId == digitalVariableId &&
                        m.MeasurementTime >= from &&
                        m.MeasurementTime <= to)
            .ToListAsync();
    }
}
