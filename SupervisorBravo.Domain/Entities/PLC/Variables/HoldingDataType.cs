namespace SupervisorBravo.Domain.Entities.PLC.Variables
{
    /// <summary>
    /// Tipo de datos para los registros de tipo holding, es tipico para decidir la logica de medicion
    /// que se utilizara.
    /// </summary>
    public enum HoldingDataType
    {
        interger,
        floating,
        doubleinterger
    }
}
