using I4_QM_app.Services;
using System;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.MockServices
{
    public class MockNotificationsService : INotificationService
    {
        public Task<bool> ShowSimpleDisplayAlert(string title, string message, string accept, string cancel)
        {
            throw new NotImplementedException();
        }

        public void ShowSimplePushNotification(int badgeNumber, string description, string title, int notificationId, string returningData)
        {
            throw new NotImplementedException();
        }
    }
}
