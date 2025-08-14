using System.ComponentModel.DataAnnotations;

public class EditVariableViewModel
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid PlcId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    public int Address { get; set; }

    public bool IsWritable { get; set; }

    // Solo para variables digitales
    public int? BitIndex { get; set; }

    // Para controlar la vista
    public bool IsDigital { get; set; }

    // Solo para variables analógicas
    public string? AnalogHoldingType { get; set; } // "integer" o "floating"
    public double ScaleFactor { get; set; } = 1.0;
}
