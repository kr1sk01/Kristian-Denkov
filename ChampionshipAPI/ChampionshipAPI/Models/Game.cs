using System;
using System.Collections.Generic;

namespace ChampionshipAPI.Models;

public partial class Game
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int GameTypeId { get; set; }

    public int GameStatusId { get; set; }

    public int BlueTeamId { get; set; }

    public int RedTeamId { get; set; }

    public int? BluePoints { get; set; }

    public int? RedPoints { get; set; }

    public DateTime? Date { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual Team BlueTeam { get; set; } = null!;

    public virtual GameStatus GameStatus { get; set; } = null!;

    public virtual GameType GameType { get; set; } = null!;

    public virtual Team RedTeam { get; set; } = null!;
}
