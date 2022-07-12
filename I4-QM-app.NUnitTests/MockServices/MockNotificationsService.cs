using I4_QM_app.Services.Notifications;
using System;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.MockServices
{
    public class MockNotificationsService : INotificationService
    {
        public Task<bool> ShowSimpleDisplayAlert(string title, string message, string accept, string cancel)
        {
            return Task.FromResult(true);
        }

        public void ShowSimplePushNotification(int badgeNumber, string description, string title, int notificationId, string returningData)
        {
            throw new NotImplementedException();
        }
    }
}
