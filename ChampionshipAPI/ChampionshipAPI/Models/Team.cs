using System;
using System.Collections.Generic;

namespace ChampionshipAPI.Models;

public partial class Team
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public byte[] Logo { get; set; } = null!;

    public int TeamTypeId { get; set; }

    public bool Active { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ICollection<Championship> Championships { get; set; } = new List<Championship>();

    public virtual ICollection<Game> GameBlueTeams { get; set; } = new List<Game>();

    public virtual ICollection<Game> GameRedTeams { get; set; } = new List<Game>();

    public virtual ICollection<TeamPlayer> TeamPlayers { get; set; } = new List<TeamPlayer>();

    public virtual TeamType TeamType { get; set; } = null!;
}
