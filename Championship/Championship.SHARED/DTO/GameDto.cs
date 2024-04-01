using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Championship.SHARED.DTO
{
    public class GameDto
    {
        [Key]
        public string Id { get; set; } = default!;

        [StringLength(255)]
        public string? Name { get; set; }

        public string? GameTypeName { get; set; }

        public string? GameStatusName { get; set; }

        public string? BlueTeamName { get; set; }

        public string? RedTeamName { get; set; }

        public int? BluePoints { get; set; }

        public int? RedPoints { get; set; }

        public string? WinnerName { get; set; }

        public string? ChampionshipName { get; set; }

        public DateTime? Date { get; set; }
    }
}
