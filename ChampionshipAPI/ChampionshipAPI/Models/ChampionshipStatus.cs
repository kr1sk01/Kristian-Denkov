using System;
using System.Collections.Generic;

namespace ChampionshipAPI.Models;

public partial class ChampionshipStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ModifiedBy { get; set; } = null!;

    public DateTime ModifiedOn { get; set; }

    public virtual ICollection<Championship> Championships { get; set; } = new List<Championship>();
}
