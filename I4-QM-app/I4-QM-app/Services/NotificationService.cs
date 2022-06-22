using Plugin.LocalNotification;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.Services
{
    public class NotificationService : INotificationService
    {
        public NotificationService()
        {
        }

        public async void ShowSimplePushNotification(int badgeNumber, string description, string title, int notificationId, string returningData)
        {
            var notification = new NotificationRequest
            {
                BadgeNumber = badgeNumber,
                Description = description,
                Title = title,
                NotificationId = notificationId,
                ReturningData = returningData
            };

            await NotificationCenter.Current.Show(notification);
        }

        public async Task<bool> ShowSimpleDisplayAlert(string title, string message, string accept, string cancel)
        {
            return await Shell.Current.DisplayAlert("Confirmation", "Start mixing now?", "Yes", "No");
        }

    }
}
