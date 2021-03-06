using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace I4_QM_app.Services.Connection
{
    /// <summary>
    /// Implementation of IConnectionService for MQTT Connection.
    /// </summary>
    public class ConnectionService : IConnectionService
    {
        private static string serverURL = "broker.hivemq.com";
        private static string baseTopicURL = "thm/sfm/sg/";

        private readonly IManagedMqttClient managedMqttClient;
        private readonly IMessageHandler ordersHandler;
        private readonly IMessageHandler additivesHandler;

        //public ConnectionService(IMessageHandler additivesHandler, IMessageHandler ordersHandler)
        //{
        //    managedMqttClient = new MqttFactory().CreateManagedMqttClient();
        //    this.additivesHandler = additivesHandler;
        //    this.ordersHandler = ordersHandler;
        //}


        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionService"/> class.
        /// </summary>
        /// <param name="ordersHandler">Orders Handler.</param>
        /// <param name="additivesHandler">Additives Handler.</param>
        public ConnectionService()
        {
            managedMqttClient = new MqttFactory().CreateManagedMqttClient();
            ordersHandler = new OrdersHandler();
            additivesHandler = new AdditivesHandler();
        }

        /// <summary>
        /// Gets a value indicating whether the mqtt client is connected.
        /// </summary>
        public bool IsConnected { get => managedMqttClient.IsConnected; }

        /// <summary>
        /// Connect MQTT Client to broker.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task ConnectClient()
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(serverURL)
                .WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce)
                .Build();

            var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(mqttClientOptions)
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(3))
                .Build();

            // get from extern file? wildcard not working???
            List<MqttTopicFilter> topics = new List<MqttTopicFilter>();
            topics.Add(new MqttTopicFilterBuilder().WithTopic(baseTopicURL + "prod/orders/add").Build());
            topics.Add(new MqttTopicFilterBuilder().WithTopic(baseTopicURL + "prod/orders/del").Build());
            topics.Add(new MqttTopicFilterBuilder().WithTopic(baseTopicURL + "prod/orders/get").Build());
            topics.Add(new MqttTopicFilterBuilder().WithTopic(baseTopicURL + "prod/orders/sync").Build());
            topics.Add(new MqttTopicFilterBuilder().WithTopic(baseTopicURL + "prod/additives/add").Build());
            topics.Add(new MqttTopicFilterBuilder().WithTopic(baseTopicURL + "prod/additives/del").Build());
            topics.Add(new MqttTopicFilterBuilder().WithTopic(baseTopicURL + "prod/additives/get").Build());
            topics.Add(new MqttTopicFilterBuilder().WithTopic(baseTopicURL + "prod/additives/sync").Build());

            await managedMqttClient.SubscribeAsync(topics);
            await managedMqttClient.StartAsync(managedMqttClientOptions);

            managedMqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessage;

            await SyncDataAsync();

            SpinWait.SpinUntil(() => managedMqttClient.PendingApplicationMessagesCount == 0, 10000);
        }

        /// <summary>
        /// Handler for received mqtt messages.
        /// </summary>
        /// <param name="eventArgs">Event args.</param>
        /// <returns>Task.</returns>
        public async Task<Task> HandleReceivedMessage(object eventArgs)
        {
            var message = ((MqttApplicationMessageReceivedEventArgs)eventArgs).ApplicationMessage;
            var topic = message.Topic;

            Console.WriteLine(topic);

            // wildcard not working???
            if (topic == baseTopicURL + "prod/orders/add"
                || topic == baseTopicURL + "prod/orders/del"
                || topic == baseTopicURL + "prod/orders/get"
                || topic == baseTopicURL + "prod/orders/sync")
            {
                await ordersHandler.HandleRoutes(message, baseTopicURL);
            }

            if (topic == baseTopicURL + "prod/additives/add"
                || topic == baseTopicURL + "prod/additives/del"
                || topic == baseTopicURL + "prod/additives/get"
                || topic == baseTopicURL + "prod/additives/sync")
            {
                await additivesHandler.HandleRoutes(message, baseTopicURL);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handler to publish messages to broker.
        /// </summary>
        /// <param name="topic">Topic.</param>
        /// <param name="message">Message.</param>
        /// <returns>Task.</returns>
        public async Task HandlePublishMessage(string topic, string message)
        {
            await managedMqttClient.EnqueueAsync(baseTopicURL + topic, message, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
        }

        /// <summary>
        /// Initial MQTT message to sync data from broker.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task SyncDataAsync()
        {
            await HandlePublishMessage("backup/orders/sync", string.Empty);
            await HandlePublishMessage("backup/additives/sync", string.Empty);
        }
    }
}