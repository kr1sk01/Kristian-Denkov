namespace ChampionshipMaster.Web.Services
{
    public class LayoutStateService
    {
        public event Action OnLayoutRefresh;

        public void RefreshLayout()
        {
            OnLayoutRefresh?.Invoke();
        }
    }
}
