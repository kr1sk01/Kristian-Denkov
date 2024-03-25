using System;
using System.Collections.Generic;

namespace ChampionshipAPI.Models;

public partial class TeamType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int TeamSize { get; set; }

    public virtual ICollection<GameType> GameTypes { get; set; } = new List<GameType>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
