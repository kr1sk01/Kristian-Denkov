using System.ComponentModel.DataAnnotations;

namespace Championship.DATA.Models
{
    public class ChampionshipType
    {
        [Key]
        public string Id { get; set; } = default!;
        //
        [StringLength(255)]
        public string? Name { get; set; }
    }
}
