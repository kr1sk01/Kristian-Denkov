using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ServiceConsoleApp;
public class Master
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public DateTime ImportDate { get; set; } = DateTime.UtcNow; // Use UtcNow for UTC timestamp

    [Required]
    public DateTime ForecastDate { get; set; } = DateTime.UtcNow;
    public virtual List<CsvData> CsvData { get; set; } = new List<CsvData>();
}
