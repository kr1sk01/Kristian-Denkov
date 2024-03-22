using ChampionshipApp.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace ChampionshipApp.Models
{
    public class Player : ApplicationUser
    {
        //public Player()
        //{
        //    Teams = new HashSet<Team>();
        //    TeamPlayers = new HashSet<TeamPlayers>();
        //}

        ////The profile picture
        //public byte[] Avatar { get; set; } = default!;
        //public bool Active { get; set; } = default!;
        //[Required]
        //public string? CreatedBy { get; set; }
        //[Required]
        //public DateTime CreatedOn { get; set; }
        //public string? ModifiedBy { get; set; }
        //public DateTime? ModifiedOn { get; set; }

        //public virtual ICollection<Team> Teams { get; set; } 
        //public virtual ICollection<TeamPlayers> TeamPlayers { get; set; }
    }
}
