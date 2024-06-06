namespace ChampionshipMaster.Web.Components.Shared
{
    public partial class CollapsiblePanel : ComponentBase
    {
        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public RenderFragment? ChildContent { get; set; }

        private bool isCollapsed = true;

        private void ToggleCollapse()
        {
            isCollapsed = !isCollapsed;
        }
    }
}
