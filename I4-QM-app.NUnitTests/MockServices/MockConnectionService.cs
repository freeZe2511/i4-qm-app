using I4_QM_app.Services.Connection;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests
{
    public class MockConnectionService : IConnectionService
    {
        public Task ConnectClient()
        {
            return Task.CompletedTask;
        }

        public Task HandlePublishMessage(string topic, string message)
        {
            return Task.CompletedTask;
        }

        public async Task<Task> HandleReceivedMessage(object eventArgs)
        {
            return Task.CompletedTask;
        }
    }
}
