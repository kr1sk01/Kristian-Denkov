using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace Championship.SHARED.DTO
{
    public class PlayerDto
    {
        public string? Id { get; set; } = default!;

        public string? Name { get; set; }

        public byte[]? Avatar { get; set; }

        public bool? Active { get; set; }

        public virtual ICollection<TeamPlayersDto> Teams { get; set; } = new HashSet<TeamPlayersDto>();
    }
}
