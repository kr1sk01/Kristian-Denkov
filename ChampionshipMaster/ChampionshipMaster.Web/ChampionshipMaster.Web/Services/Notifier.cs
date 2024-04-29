using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Radzen;

namespace ChampionshipMaster.Web.Services
{
    public class Notifier : INotifier
    {

        private readonly NotificationService _notificationService;

        public Notifier(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        public void SendErrorNotification(string message, int duration = 3)
        {        
            _notificationService.Notify(new NotificationMessage
            {
                Detail = message,
                Severity = NotificationSeverity.Error,
                Duration = duration * 1000
                
            });
        }

        public void SendWarningNotification(string message, int duration = 3)
{
            _notificationService.Notify(new NotificationMessage
            {
                Detail = message,
                Severity = NotificationSeverity.Warning,
                Duration = duration * 1000
            });
        }

        public void SendInformationalNotification(string message, int duration = 3)
{
            _notificationService.Notify(new NotificationMessage
            {
                Detail = message,
                Severity = NotificationSeverity.Info,
                Duration = duration * 1000
            });
            
        }

        public void SendSuccessNotification(string message, int duration = 3)
{
            _notificationService.Notify(new NotificationMessage
            {
                Detail = message,
                Severity = NotificationSeverity.Success,
                Duration = duration * 1000
            });

        }
    }
}
