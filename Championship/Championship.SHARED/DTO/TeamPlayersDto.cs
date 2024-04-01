using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.SHARED.DTO
{
    public class TeamPlayersDto
    {
        [Key]
        public string Id { get; set; } = default!;

        public string? PlayerName { get; set; }
        public virtual PlayerDto? Player { get; set; }

        public string? TeamName { get; set; }
        public virtual TeamDto? Team { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}
