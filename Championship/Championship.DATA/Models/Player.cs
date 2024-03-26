using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Championship.DATA.Models
{
    public class Player : IdentityUser
    {
        public byte[]? Avatar { get; set; } = default;

        public bool? Active { get; set; } = null;

        public string? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; } = null;

        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<TeamPlayers> Teams { get; set; } = new HashSet<TeamPlayers>();
    }
}
