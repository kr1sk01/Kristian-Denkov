namespace ChampionshipMaster.Web.Components.Shared
{
    public partial class ChampionshipBracket : ComponentBase
    {
        [Inject] IClientLotService lotService { get; set; } = default!;

        [Parameter] public required ICollection<GameDto> Games { get; set; } = [];
        [Parameter] public int TeamsCount { get; set; }

        GameDto[][] gamesGrid = [];
        int rounds;

        protected override void OnInitialized()
        {
            Games.OrderBy(x => x.Id);
            rounds = lotService.Rounds(TeamsCount);
            gamesGrid = new GameDto[rounds][];

            int gamesToSkip = 0;
            for (int i = 1; i <= rounds; i++)
            {
                int gamesInRound = lotService.GamesInRound(TeamsCount, i);
                gamesGrid[i - 1] = new GameDto[gamesInRound];

                for (int j = 0; j < gamesInRound; j++)
                {
                    gamesGrid[i - 1][j] = Games.Skip(gamesToSkip++).First();
                }
            }



            base.OnInitialized();
        }
    }
}
