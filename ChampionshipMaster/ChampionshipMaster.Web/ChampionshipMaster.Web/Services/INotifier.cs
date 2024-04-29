namespace ChampionshipMaster.Web.Services
{
    /// <summary>
    /// Represents an interface for notifying users.
    /// </summary>
    public interface INotifier
    {
        /// <summary>
        /// Sends an error notification with the specified message.
        /// </summary>
        /// <param name="message">The notification message.</param>
        /// <param name="duration">The duration of the notification message in seconds (default is 3 seconds).</param>
        void SendErrorNotification(string message, int duration = 3);

        /// <summary>
        /// Sends a warning notification with the specified message.
        /// </summary>
        /// <param name="message">The notification message.</param>
        /// <param name="duration">The duration of the notification message in seconds (default is 3 seconds).</param>
        void SendWarningNotification(string message, int duration = 3);

        /// <summary>
        /// Sends an informational notification with the specified message.
        /// </summary>
        /// <param name="message">The notification message.</param>
        /// <param name="duration">The duration of the notification message in seconds (default is 3 seconds).</param>
        void SendInformationalNotification(string message, int duration = 3);

        /// <summary>
        /// Sends a success notification with the specified message.
        /// </summary>
        /// <param name="message">The notification message.</param>
        /// <param name="duration">The duration of the notification message in seconds (default is 3 seconds).</param>
        void SendSuccessNotification(string message, int duration = 3);
    }
}
