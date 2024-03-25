using System;
using System.Collections.Generic;

namespace ChampionshipAPI.Models;

public partial class Championship
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public byte[]? Logo { get; set; }

    public int ChampionshipTypeId { get; set; }

    public int ChampionshipStatusId { get; set; }

    public int GameTypeId { get; set; }

    public int? WinnerId { get; set; }

    public DateTime? LotDate { get; set; }

    public DateTime? Date { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ChampionshipStatus ChampionshipStatus { get; set; } = null!;

    public virtual ChampionshipType ChampionshipType { get; set; } = null!;

    public virtual GameType GameType { get; set; } = null!;

    public virtual Team? Winner { get; set; }
}
