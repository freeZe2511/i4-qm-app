using I4_QM_app.Services.Connection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    public class ConnectionService : IConnectionService
    {
        private static string serverURL = "broker.hivemq.com";
        private static string baseTopicURL = "thm/sfm/sg/";
        private readonly IManagedMqttClient managedMqttClient;
        private readonly IMessageHandler ordersHandler;
        private readonly IMessageHandler additivesHandler;

        public ConnectionService()
        {
            managedMqttClient = new MqttFactory().CreateManagedMqttClient();
            ordersHandler = new OrdersHandler();
            additivesHandler = new AdditivesHandler();
        }

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

            SyncDataAsync();

            managedMqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessage;

            SpinWait.SpinUntil(() => managedMqttClient.PendingApplicationMessagesCount == 0, 10000);

        }

        private async void SyncDataAsync()
        {
            await HandlePublishMessage("backup/orders/sync", string.Empty);
            await HandlePublishMessage("backup/additives/sync", string.Empty);
        }

        public bool IsConnected { get => managedMqttClient.IsConnected; }

        public async void ToggleMqttClient()
        {
            //    Console.WriteLine(managedMqttClient.IsConnected);
            //    Console.WriteLine(IsConnected);

            //    if (managedMqttClient.IsConnected)
            //    {
            //        await managedMqttClient.StopAsync();
            //    }
            //    else
            //    {
            //        
            //    }
        }

        public void UpdateBrokerURL(string newURL)
        {
            serverURL = newURL;
        }

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

        public async Task HandlePublishMessage(string topic, string message)
        {
            await managedMqttClient.EnqueueAsync(baseTopicURL + topic, message, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
        }

    }

}