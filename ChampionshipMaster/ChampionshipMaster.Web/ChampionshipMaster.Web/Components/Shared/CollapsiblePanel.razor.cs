namespace ChampionshipMaster.Web.Components.Shared
{
    public partial class CollapsiblePanel : ComponentBase
    {
        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public RenderFragment? ChildContent { get; set; }

        private bool isCollapsed = true;
        private bool isBreathing = false;

        private void HandleClick()
        {
            isCollapsed = !isCollapsed;
            isBreathing = true;
            StateHasChanged(); // Ensure the component re-renders to apply the class
            InvokeAsync(async () =>
            {
                await Task.Delay(500); // Duration of the animation
                isBreathing = false;
                StateHasChanged(); // Ensure the component re-renders to remove the class
            });
        }
    }
}
