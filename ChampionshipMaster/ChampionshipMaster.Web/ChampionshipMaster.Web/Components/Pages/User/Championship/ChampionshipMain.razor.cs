using System.IdentityModel.Tokens.Jwt;

namespace ChampionshipMaster.Web.Components.Pages.User.Championship
{
    public partial class ChampionshipMain : ComponentBase
    {
        [Inject]
        ITokenService tokenService { get; set; } = default!;
        [Inject]
        IHttpClientFactory httpClient { get; set; } = default!;
        [Inject]
        INotifier notifier { get; set; } = default!;
        [Inject]
        IConfiguration configuration { get; set; } = default!;
        [Inject]
        NavigationManager NavigationManager { get; set; } = default!;

        RadzenDataGrid<ChampionshipDto>? championshipList;

        string StatusColor = "white";

        private void Sort()
        {
            if (championships != null)
            {
                championships = championships.OrderBy(x => x.Name).ToList();
                StateHasChanged();
            }
        }
        private List<ChampionshipDto>? championships;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (await tokenService.ValidateToken())
                {
                    var token = new JwtSecurityTokenHandler().ReadJwtToken(await tokenService.GetToken());

                    StateHasChanged();
                }
                else { NavigationManager.NavigateTo("/login"); }

                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                var test = await client.GetFromJsonAsync<List<ChampionshipDto>>("api/championship/details");
                if (test != null)
                    championships = test;
                StateHasChanged();
            }
        }

        async Task OpenChampionship(string id)
        {
            NavigationManager.NavigateTo($"/championshipDetails/{id}");
        }
    }
}
