using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.SHARED.DTO
{
    public class ChampionshipClassDto
    {
        
        public int Id { get; set; }

        
        public string? Name { get; set; }

        public byte[]? Logo { get; set; }

        public string? ChampionshipTypeName { get; set; }

        public string? ChampionshipStatusName { get; set; }

        public string? GameTypeName { get; set; }

        public string? WinnerName { get; set; }

        public DateTime? LotDate { get; set; }

        public DateTime? Date { get; set; }

        public virtual ICollection<GameDto> Games { get; set; } = new HashSet<GameDto>();

        public virtual ICollection<TeamDto> Teams { get; set; } = new HashSet<TeamDto>();
    }
}
