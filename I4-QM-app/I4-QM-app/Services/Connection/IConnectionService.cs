using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    public interface IConnectionService
    {
        Task ConnectClient();
        Task<Task> HandleReceivedMessage(object eventArgs);
        Task HandlePublishMessage(string topic, string message);

    }
}
