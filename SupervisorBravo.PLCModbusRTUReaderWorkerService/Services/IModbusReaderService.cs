namespace SupervisorBravo.PLCModbusRTUReaderWorkerService.Services
{
    public interface IModbusReaderService
    {
        Task<bool> ReadCoilAsync(byte unitId, ushort address);
        /// <summary>
        /// Método para leer Registros Holding, tener en cuenta que luego debe seleccionarse el bit
        /// especifico que se debe trabajar en caso de que sea una variable digital
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<ushort[]> ReadHoldingRegisterAsync(byte unitId, ushort address);
    }

}
