using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.Services
{
    /// <summary>
    /// Implementation of INotificationService.
    /// </summary>
    public class NotificationService : INotificationService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationService"/> class.
        /// </summary>
        public NotificationService()
        {
            NotificationCenter.Current.NotificationTapped += LoadPageFromNotification;
        }

        /// <summary>
        /// Show simple push notification.
        /// </summary>
        /// <param name="badgeNumber">Badge Number.</param>
        /// <param name="description">Description.</param>
        /// <param name="title">Title.</param>
        /// <param name="notificationId">Notification ID.</param>
        /// <param name="returningData">Returning Data.</param>
        public async void ShowSimplePushNotification(int badgeNumber, string description, string title, int notificationId, string returningData)
        {
            var notification = new NotificationRequest
            {
                BadgeNumber = badgeNumber,
                Description = description,
                Title = title,
                NotificationId = notificationId,
                ReturningData = returningData,
            };

            await NotificationCenter.Current.Show(notification);
        }

        /// <summary>
        /// Show simple display alert.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="message">Message.</param>
        /// <param name="accept">Accept option.</param>
        /// <param name="cancel">Cancel option.</param>
        /// <returns>Task.</returns>
        public async Task<bool> ShowSimpleDisplayAlert(string title, string message, string accept, string cancel)
        {
            return await Shell.Current.DisplayAlert(title, message, accept, cancel);
        }

        /// <summary>
        /// Load app page from returning data from push notification.
        /// </summary>
        /// <param name="e">NotificationEventArgs.</param>
        // TODO 
        private void LoadPageFromNotification(NotificationEventArgs e)
        {
            var data = e.Request.ReturningData;

            if (string.IsNullOrWhiteSpace(data))
            {
                return;
            }

            Page page = null;
            if (data == "OrdersPage")
            {
                page = new Views.OrdersPage();
            }

            Shell.Current.Navigation.PushAsync(page);
        }
    }
}
