using System;
using System.Collections.Generic;

namespace ChampionshipAPI.Models;

public partial class ChampionshipType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Championship> Championships { get; set; } = new List<Championship>();
}
