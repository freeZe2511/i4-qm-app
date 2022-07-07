using MQTTnet;
using System.Threading.Tasks;

namespace I4_QM_app.Services.Connection
{
    /// <summary>
    /// Interface to handle mqtt topics as routes.
    /// </summary>
    public interface IMessageHandler
    {
        /// <summary>
        /// Handler to redirect to specific method per route.
        /// </summary>
        /// <param name="message">Mqtt message.</param>
        /// <param name="baseTopicURL">mqtt base url.</param>
        /// <returns>Task.</returns>
        Task HandleRoutes(MqttApplicationMessage message, string baseTopicURL);

        /// <summary>
        /// Handles add topic/route.
        /// </summary>
        /// <param name="message">Mqtt message.</param>
        /// <returns>Task.</returns>
        Task HandleAddRoute(MqttApplicationMessage message);

        /// <summary>
        /// Handles get topic/route.
        /// </summary>
        /// <param name="message">Mqtt message.</param>
        /// <returns>Task.</returns>
        Task HandleGetRoute(MqttApplicationMessage message);

        /// <summary>
        /// Handles update topic/route.
        /// </summary>
        /// <param name="message">Mqtt message.</param>
        /// <returns>Task.</returns>
        Task HandleUpdateRoute(MqttApplicationMessage message);

        /// <summary>
        /// Handles delete topic/route.
        /// </summary>
        /// <param name="message">Mqtt message.</param>
        /// <returns>Task.</returns>
        Task HandleDelRoute(MqttApplicationMessage message);
    }
}
