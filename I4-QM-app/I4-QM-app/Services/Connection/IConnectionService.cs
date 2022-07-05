using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    /// <summary>
    /// Interface for connection services.
    /// </summary>
    public interface IConnectionService
    {
        /// <summary>
        /// Connect client to server.
        /// </summary>
        /// <returns>Task.</returns>
        Task ConnectClient();

        /// <summary>
        /// Handle received messages from server.
        /// </summary>
        /// <param name="eventArgs">Event Args.</param>
        /// <returns>Task.</returns>
        Task<Task> HandleReceivedMessage(object eventArgs);

        /// <summary>
        /// Handle publishing messages to server.
        /// </summary>
        /// <param name="topic">Topic.</param>
        /// <param name="message">Message.</param>
        /// <returns>Task.</returns>
        Task HandlePublishMessage(string topic, string message);
    }
}
