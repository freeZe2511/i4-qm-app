using MQTTnet;
using System.Threading.Tasks;

namespace I4_QM_app.Services.Connection
{
    public interface IMessageHandler
    {
        Task HandleRoutes(MqttApplicationMessage message, string baseTopicURL);

        Task HandleAddRoute(MqttApplicationMessage message);

        Task HandleGetRoute(MqttApplicationMessage message);

        Task HandleUpdateRoute(MqttApplicationMessage message);

        Task HandleDelRoute(MqttApplicationMessage message);
    }
}
