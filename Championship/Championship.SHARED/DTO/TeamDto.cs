using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Championship.SHARED.DTO
{
    public class TeamDto
    {
        
        public int Id { get; set; }

        public string? Name { get; set; }

        public byte[]? Logo { get; set; }

        public string? TeamTypeName { get; set; }

        public bool? Active { get; set; }

        public virtual ICollection<PlayerDto> Players { get; set; } = new HashSet<PlayerDto>();

        //public virtual ICollection<ChampionshipClassDto> Championships { get; set; } = new HashSet<ChampionshipClassDto>();
    }
}
