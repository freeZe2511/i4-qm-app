using I4_QM_app.Services;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests
{
    public class MockConnectionService : IConnectionService
    {
        public Task ConnectClient()
        {
            throw new System.NotImplementedException();
        }

        public Task HandlePublishMessage(string topic, string message)
        {
            throw new System.NotImplementedException();
        }

        public Task<Task> HandleReceivedMessage(object eventArgs)
        {
            throw new System.NotImplementedException();
        }
    }
}
