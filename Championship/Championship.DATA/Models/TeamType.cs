using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.DATA.Models
{
    public class TeamType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        //
        [StringLength(255)]
        public string? Name { get; set; }
        //
        public int? TeamSize { get; set; }
    }
}
