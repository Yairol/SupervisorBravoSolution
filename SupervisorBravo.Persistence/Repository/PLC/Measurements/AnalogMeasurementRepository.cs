using Microsoft.EntityFrameworkCore;

namespace SupervisorBravo.Persistence.Repository;

public partial class AplicationRepository : IAnalogMeasurementRepository
{
    private ApplicationDbContext EnsureContext()
    {
        if (_context is null)
            throw new InvalidOperationException("No hay un contexto activo. Asegúrate de iniciar una transacción o crear el contexto antes de usar el repositorio.");
        return _context;
    }

    public async Task<List<AnalogMeasurement>> GetAllAnalogMeasurementsAsync()
    {
        var ctx = EnsureContext();
        return await ctx.Set<AnalogMeasurement>().ToListAsync();
    }

    public async Task<AnalogMeasurement?> GetAnalogMeasurementByIdAsync(Guid id)
    {
        var ctx = EnsureContext();
        return await ctx.Set<AnalogMeasurement>().FindAsync(id);
    }

    public async Task AddAnalogMeasurementAsync(AnalogMeasurement measurement)
    {
        var ctx = EnsureContext();
        await ctx.Set<AnalogMeasurement>().AddAsync(measurement);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateAnalogMeasurementAsync(AnalogMeasurement measurement)
    {
        var ctx = EnsureContext();
        ctx.Set<AnalogMeasurement>().Update(measurement);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteAnalogMeasurementAsync(Guid id)
    {
        var ctx = EnsureContext();
        var entry = await ctx.Set<AnalogMeasurement>().FindAsync(id);
        if (entry is not null)
        {
            ctx.Set<AnalogMeasurement>().Remove(entry);
            await ctx.SaveChangesAsync();
        }
    }

    public async Task<List<AnalogMeasurement>> GetAnalogMeasurementsByVariableIdAsync(Guid analogVariableId)
    {
        var ctx = EnsureContext();
        return await ctx.Set<AnalogMeasurement>()
            .Where(m => m.PLCAnalogVariableId == analogVariableId)
            .ToListAsync();
    }

    public async Task<List<AnalogMeasurement>> GetAnalogMeasurementsByTimeRangeAsync(Guid analogVariableId, DateTime from, DateTime to)
    {
        if (from > to)
        {
            (from, to) = (to, from);
        }

        var ctx = EnsureContext();
        return await ctx.Set<AnalogMeasurement>()
            .Where(m => m.PLCAnalogVariableId == analogVariableId &&
                        m.MeasurementTime >= from &&
                        m.MeasurementTime <= to)
            .ToListAsync();
    }
}
