using System.ComponentModel.DataAnnotations;

namespace Championship.SHARED.DTO
{
    public class ChampionshipTypeDto
    {
        [Key]
        public string Id { get; set; } = default!;
        //
        [StringLength(255)]
        public string? Name { get; set; }
    }
}
