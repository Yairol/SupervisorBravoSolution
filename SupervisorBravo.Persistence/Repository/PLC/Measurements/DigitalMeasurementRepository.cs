using Microsoft.EntityFrameworkCore;

namespace SupervisorBravo.Persistence.Repository;

public partial class AplicationRepository : IDigitalMeasurementRepository
{
    public async Task<List<DigitalMeasurement>> GetAllDigitalMeasurementsAsync()
    {
        return await _context.Set<DigitalMeasurement>().ToListAsync();
    }

    public async Task<DigitalMeasurement?> GetDigitalMeasurementByIdAsync(Guid id)
    {
        return await _context.Set<DigitalMeasurement>().FindAsync(id);
    }

    public async Task AddDigitalMeasurementAsync(DigitalMeasurement measurement)
    {
        await _context.Set<DigitalMeasurement>().AddAsync(measurement);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateDigitalMeasurementAsync(DigitalMeasurement measurement)
    {
        _context.Set<DigitalMeasurement>().Update(measurement);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDigitalMeasurementAsync(Guid id)
    {
        var entry = await _context.Set<DigitalMeasurement>().FindAsync(id);
        if (entry is not null)
        {
            _context.Set<DigitalMeasurement>().Remove(entry);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<DigitalMeasurement>> GetDigitalMeasurementsByVariableIdAsync(Guid digitalVariableId)
    {
        return await _context.Set<DigitalMeasurement>()
            .Where(m => m.PLCDigitalVariableId == digitalVariableId)
            .ToListAsync();
    }

    public async Task<List<DigitalMeasurement>> GetDigitalMeasurementsByTimeRangeAsync(Guid digitalVariableId, DateTime from, DateTime to)
    {
        return await _context.Set<DigitalMeasurement>()
            .Where(m => m.PLCDigitalVariableId == digitalVariableId &&
                        m.MeasurementTime >= from && m.MeasurementTime <= to)
            .ToListAsync();
    }
}
