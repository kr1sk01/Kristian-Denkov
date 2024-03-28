using System.ComponentModel.DataAnnotations;

namespace Championship.SHARED.DTO
{
    public class TeamTypeDto
    {
        [Key]
        public string Id { get; set; } = default!;
        //
        [StringLength(255)]
        public string? Name { get; set; }
        //
        public int? TeamSize { get; set; }

        public virtual ICollection<TeamDto> Teams { get; set; } = new HashSet<TeamDto>();
    }
}
