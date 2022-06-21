using Plugin.LocalNotification;

namespace I4_QM_app.Services
{
    public class NotificationService
    {
        public NotificationService()
        {

        }

        public async void ShowSimpleNotification(int badgeNumber, string description, string title, int notificationId, string returningData)
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

    }
}
