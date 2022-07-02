using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.Services
{
    public class NotificationService : INotificationService
    {
        public NotificationService()
        {
            NotificationCenter.Current.NotificationTapped += LoadPageFromNotification;
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
            return await Shell.Current.DisplayAlert(title, message, accept, cancel);
        }

        // TODO
        private void LoadPageFromNotification(NotificationEventArgs e)
        {
            var data = e.Request.ReturningData;

            if (string.IsNullOrWhiteSpace(data))
            {
                return;
            }

            // TODO
            Page page = null;
            if (data == "OrdersPage") page = new Views.OrdersPage();

            Shell.Current.Navigation.PushAsync(page);

            //await Shell.Current.GoToAsync($"//{nameof(OrdersPage)}");


        }

    }
}
