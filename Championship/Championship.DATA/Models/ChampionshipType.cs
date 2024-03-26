using System.ComponentModel.DataAnnotations;

namespace Championship.DATA.Models
{
    public class ChampionshipType
    {
        [Key]
        public int Id { get; set; }
        //
        [StringLength(255)]
        public string? Name { get; set; }
    }
}
