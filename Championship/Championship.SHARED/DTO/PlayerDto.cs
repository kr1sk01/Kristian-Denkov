using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Championship.SHARED.DTO
{
    public class PlayerDto
    {

   
        public byte[]? Avatar { get; set; } = default;

        public bool? Active { get; set; } = null;

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; } = null;

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<TeamPlayersDto> Teams { get; set; } = new HashSet<TeamPlayersDto>();
    }
}
