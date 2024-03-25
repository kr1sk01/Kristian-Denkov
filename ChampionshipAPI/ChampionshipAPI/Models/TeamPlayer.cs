using System;
using System.Collections.Generic;

namespace ChampionshipAPI.Models;

public partial class TeamPlayer
{
    public int Id { get; set; }

    public string PlayerId { get; set; } = null!;

    public int TeamId { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    //public virtual AspNetUser Player { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;
}
