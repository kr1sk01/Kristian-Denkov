using System;
using System.Collections.Generic;

namespace ChampionshipAPI.Models;

public partial class GameType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int MaxPoints { get; set; }

    public int TeamTypeId { get; set; }

    public virtual ICollection<Championship> Championships { get; set; } = new List<Championship>();

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    public virtual TeamType TeamType { get; set; } = null!;
}
