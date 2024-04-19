using Microsoft.AspNetCore.SignalR;

namespace ChampionshipMaster.API
{
    public class NotificationsHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            Clients.Client(Context.ConnectionId).ReceiveNotification($"Thanks for connecting to the app!");

            await base.OnConnectedAsync();
        }
    }
}
public interface INotificationClient
{
    Task ReceiveNotification(string message);
}