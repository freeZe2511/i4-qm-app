using System.Threading.Tasks;

namespace I4_QM_app.Services.Notifications
{
    /// <summary>
    /// Interface for notifications.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Show simple push notifications.
        /// </summary>
        /// <param name="badgeNumber">Badge Number.</param>
        /// <param name="description">Description.</param>
        /// <param name="title">Title.</param>
        /// <param name="notificationId">Notification ID.</param>
        /// <param name="returningData">Returning Data.</param>
        void ShowSimplePushNotification(int badgeNumber, string description, string title, int notificationId, string returningData);

        /// <summary>
        /// Show simple display alert.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="accept">Accept option.</param>
        /// <param name="cancel">Cancel option.</param>
        /// <returns>Task.</returns>
        Task<bool> ShowSimpleDisplayAlert(string title, string message, string accept, string cancel);
    }
}
