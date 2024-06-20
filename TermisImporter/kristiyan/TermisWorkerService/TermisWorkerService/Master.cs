using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TermisWorkerService;
public class Master
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime ImportDate { get; set; } = DateTime.UtcNow; // Use UtcNow for UTC timestamp

    [Required]
    public DateTime ForecastDate { get; set; } = DateTime.UtcNow;
    [Required]
    public string Status { get; set; } = default!;
    public virtual List<CsvData> CsvData { get; set; } = new List<CsvData>();
}
