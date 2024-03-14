namespace SomeBlazorApp.Services;

public class AppStateService
{
    public int currentCount { get; set; } = 0;

    public int upgrades { get; set; } = 0;

    public int incrementAmount { get; set; } = 1;

    public int price { get; set; } = 10;
    public bool hasMoney { get; set; } = true;
}
