namespace SupervisorBravo.WorkerService.Utilities
{
    public interface IModbusService
    {
        Task<ushort[]> ReadRegistersAsync(byte slaveId, ushort startAddress, ushort numberOfPoints);


    }
}
