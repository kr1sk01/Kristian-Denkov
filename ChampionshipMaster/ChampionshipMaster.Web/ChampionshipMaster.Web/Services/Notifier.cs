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

        public void SendErrorMessage(string message)
        {        
            _notificationService.Notify(new NotificationMessage
            {
                Detail = message,
                Severity = NotificationSeverity.Error
            });
        }
        public void SendWarningMessage(string message)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Detail = message,
                Severity = NotificationSeverity.Warning
            });
        }
        public void SendInformationalMessage(string message)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Detail = message,
                Severity = NotificationSeverity.Info
            });
            
        }
        public void SendSuccessMessage(string message)
        {
            _notificationService.Notify(new NotificationMessage
            {
                Detail = message,
                Severity = NotificationSeverity.Success
            });

        }
    }
}
