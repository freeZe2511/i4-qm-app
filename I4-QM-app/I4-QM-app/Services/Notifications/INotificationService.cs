using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    public interface INotificationService
    {
        void ShowSimplePushNotification(int badgeNumber, string description, string title, int notificationId, string returningData);
        Task<bool> ShowSimpleDisplayAlert(string title, string message, string accept, string cancel);

    }
}
