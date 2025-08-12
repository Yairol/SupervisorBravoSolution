using Microsoft.EntityFrameworkCore;

namespace SupervisorBravo.Persistence.Repository;

public partial class AplicationRepository : IAnalogMeasurementRepository
{
    public async Task<List<AnalogMeasurement>> GetAllAnalogMeasurementsAsync()
    {
        return await _context.Set<AnalogMeasurement>().ToListAsync();
    }

    public async Task<AnalogMeasurement?> GetAnalogMeasurementByIdAsync(Guid id)
    {
        return await _context.Set<AnalogMeasurement>().FindAsync(id);
    }

    public async Task AddAnalogMeasurementAsync(AnalogMeasurement measurement)
    {
        await _context.Set<AnalogMeasurement>().AddAsync(measurement);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAnalogMeasurementAsync(AnalogMeasurement measurement)
    {
        _context.Set<AnalogMeasurement>().Update(measurement);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAnalogMeasurementAsync(Guid id)
    {
        var entry = await _context.Set<AnalogMeasurement>().FindAsync(id);
        if (entry is not null)
        {
            _context.Set<AnalogMeasurement>().Remove(entry);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<AnalogMeasurement>> GetAnalogMeasurementsByVariableIdAsync(Guid analogVariableId)
    {
        return await _context.Set<AnalogMeasurement>()
            .Where(m => m.PLCAnalogVariableId == analogVariableId)
            .ToListAsync();
    }

    public async Task<List<AnalogMeasurement>> GetAnalogMeasurementsByTimeRangeAsync(Guid analogVariableId, DateTime from, DateTime to)
    {
        if (from > to)
        {
            DateTime save = to;
            to = from;
            from = save;
        }
        return await _context.Set<AnalogMeasurement>()
            .Where(m => m.PLCAnalogVariableId == analogVariableId &&
                        m.MeasurementTime >= from && m.MeasurementTime <= to)
            .ToListAsync();
    }
}
