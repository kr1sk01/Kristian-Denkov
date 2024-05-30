using ChampionshipMaster.DATA.Models;
using ChampionshipMaster.Web.Components.Pages.User.Game;
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
        IImageService imageService { get; set; } = default!;
        [Inject]
        NavigationManager NavigationManager { get; set; } = default!;
        [Inject] DialogService DialogService { get; set; } = default!;
        [Inject] ContextMenuService ContextMenuService { get; set; } = default!;
        RadzenDataGrid<ChampionshipDto>? championshipList;

        string StatusColor = "white";
        IList<ChampionshipDto>? selectedChampionship;


        public bool isAdmin = false;
        public bool isLogged = false;

        bool disabledEdit = true;
        bool disabledJoin = true;
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
                if(await tokenService.ValidateToken(true))
                {
                    isAdmin = true;
                }
                using HttpClient client = httpClient.CreateClient(configuration["ClientName"]!);
                var test = await client.GetFromJsonAsync<List<ChampionshipDto>>("api/championship/details");
                if (test != null)
                    championships = test;
                StateHasChanged();
            }
        }
        void Update(DataGridRowMouseEventArgs<ChampionshipDto> args)
        {
            if (args != null && args.Data.ChampionshipStatusName == "Open")
                disabledJoin = false;
            else
                disabledJoin = true;

            if (args != null)
                disabledEdit = false;
            else
                disabledEdit = true;

            StateHasChanged();
        }
        void OnCellContextMenu(DataGridCellMouseEventArgs<ChampionshipDto> args)
        {
            if (args == null)
                return;
            if (args.Data == null)
                return;
            if (args.Data != null && args.Data.ChampionshipStatusName == "Open")
            {
                selectedChampionship = new List<ChampionshipDto>() { args.Data };
                if (isAdmin)
                {
                    ContextMenuService.Open(args,
                                       new List<ContextMenuItem> {
                        new ContextMenuItem(){ Text = "Details", Value = 1, Icon = "info" },
                        new ContextMenuItem(){ Text = "Edit", Value = 1, Icon = "edit" },
                        new ContextMenuItem(){ Text = "Join", Value = 1, Icon = "groups" },
                                                                       },
                                       async (e) =>
                                       {
                                           if (e.Text == "Details")
                                           {
                                               OpenChampionship(args.Data.Id.ToString());
                                               ContextMenuService.Close();
                                           }
                                           if (e.Text == "Edit")
                                           {
                                               OpenChampionship(args.Data.Id.ToString());
                                               ContextMenuService.Close();
                                           }
                                           if (e.Text == "Join")
                                           {
                                               await OpenJoinDialog(args.Data.Id.ToString(), args.Data.Name.ToString());
                                               ContextMenuService.Close();
                                           }
                                       });
                }
                else
                {
                    ContextMenuService.Open(args,
                   new List<ContextMenuItem> {
                        new ContextMenuItem(){ Text = "Details", Value = 1, Icon = "info" },
                        new ContextMenuItem(){ Text = "Join", Value = 1, Icon = "groups" },
                                                   },
                   async (e) =>
                   {
                       if (e.Text == "Details")
                       {
                           OpenChampionship(args.Data.Id.ToString());
                           ContextMenuService.Close();
                       }
                       if (e.Text == "Join")
                       {
                           await OpenJoinDialog(args.Data.Id.ToString(), args.Data.Name.ToString());
                           ContextMenuService.Close();
                       }
                   });
                }


            }
            else
            {
                selectedChampionship = new List<ChampionshipDto>() { args.Data };
                if (isAdmin)
                {
                    ContextMenuService.Open(args,
                                       new List<ContextMenuItem> {
                        new ContextMenuItem(){ Text = "Details", Value = 1, Icon = "info" },
                        new ContextMenuItem(){ Text = "Edit", Value = 1, Icon = "edit" },
                                                                       },
                                       async (e) =>
                                       {
                                           if (e.Text == "Details")
                                           {
                                               OpenChampionship(args.Data.Id.ToString());
                                               ContextMenuService.Close();
                                           }
                                           if (e.Text == "Edit")
                                           {
                                               OpenChampionship(args.Data.Id.ToString());
                                               ContextMenuService.Close();
                                           }
                                       });
                }
                else
                {
                    ContextMenuService.Open(args,
                   new List<ContextMenuItem> {
                        new ContextMenuItem(){ Text = "Details", Value = 1, Icon = "info" },
                                                   },
                   async (e) =>
                   {
                       if (e.Text == "Details")
                       {
                           OpenChampionship(args.Data.Id.ToString());
                           ContextMenuService.Close();
                       }
                   });
                }
            }


        }

        void OpenChampionship(string id)
        {
            NavigationManager.NavigateTo($"/championshipDetails/{id}");
        }
        public async Task OpenJoinDialog(string championshipId, string championshipName) 
        {
            await DialogService.OpenAsync<JoinChampionship>($"",
                       new Dictionary<string, object>() { 
                           { "championshipId", championshipId }, 
                           { "championshipName",  championshipName} 
                       },
                       new DialogOptions() { Width = "45%", Height = "60%", Draggable = true, CloseDialogOnEsc = true });
        }
        public async Task OpenChampionshipCreatePage()
        {
            await DialogService.OpenAsync<CreateChampionship>($"",
                   new Dictionary<string, object>() { },
                   new DialogOptions() { Width = "45%", Height = "60%", Draggable = true, CloseDialogOnEsc = true });
        }
    }
}
