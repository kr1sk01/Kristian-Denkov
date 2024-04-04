namespace ChampionshipMaster.SHARED.DTO;

public class PlayerDto
{
    public string? Id { get; set; } = default!;

    public string? Name { get; set; }

    public byte[]? Avatar { get; set; }

    public bool? Active { get; set; }

    public virtual ICollection<TeamDto> Teams { get; set; } = new HashSet<TeamDto>();
}
