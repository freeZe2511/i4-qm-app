using I4_QM_app.Models;
using I4_QM_app.Services.Connection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            additivesHandler = new OrdersHandler();
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

            // get from extern?
            List<MqttTopicFilter> topics = new List<MqttTopicFilter>();
            topics.Add(new MqttTopicFilterBuilder().WithTopic(baseTopicURL + "prod/orders/add").Build());
            topics.Add(new MqttTopicFilterBuilder().WithTopic(baseTopicURL + "prod/orders/del").Build());
            topics.Add(new MqttTopicFilterBuilder().WithTopic(baseTopicURL + "prod/orders/get").Build());
            topics.Add(new MqttTopicFilterBuilder().WithTopic(baseTopicURL + "additives/sync").Build());
            topics.Add(new MqttTopicFilterBuilder().WithTopic(baseTopicURL + "images/+/x").Build());

            await managedMqttClient.SubscribeAsync(topics);
            await managedMqttClient.StartAsync(managedMqttClientOptions);

            managedMqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessage;

            SpinWait.SpinUntil(() => managedMqttClient.PendingApplicationMessagesCount == 0, 10000);

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

            // wildcard not working???
            if (topic == baseTopicURL + "prod/orders/add"
                || topic == baseTopicURL + "prod/orders/del"
                || topic == baseTopicURL + "prod/orders/get")
            {
                await ordersHandler.HandleRoutes(message, baseTopicURL);
            }
            if (topic == baseTopicURL + "additives/sync") await HandleSyncAdditives(message);

            return Task.CompletedTask;
        }

        public async Task HandlePublishMessage(string topic, string message)
        {
            await managedMqttClient.EnqueueAsync(baseTopicURL + topic, message, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
        }

        private async Task HandleSyncAdditives(MqttApplicationMessage message)
        {
            Console.WriteLine(message.Payload);
            string req = Encoding.UTF8.GetString(message.Payload);

            Console.WriteLine($"+ Sync Additives");

            List<Additive> additives = JsonConvert.DeserializeObject<List<Additive>>(req);
            await App.AdditivesDataService.DeleteAllItemsAsync();

            int additivesCount = 0;

            foreach (Additive additive in additives)
            {
                // TODO image id
                if (additive.Id == null || additive.Name == null)
                {
                    continue;
                }

                additive.ActualPortion = 0;
                additive.Amount = 0;
                additive.Portion = 0;
                additive.Checked = false;

                if (!String.IsNullOrEmpty(additive.ImageBase64))
                {
                    //var img = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(additive.ImageBase64)));

                    var fs = App.DB.GetStorage<string>("myImages");
                    fs.Upload(additive.Id, additive.Name, new MemoryStream(Convert.FromBase64String(additive.ImageBase64)));
                }
                else
                {
                    // TODO standard bild?
                    Console.WriteLine("no pic");
                }

                //Console.WriteLine(additive.ImageBase64);

                await App.AdditivesDataService.AddItemAsync(additive);
                additivesCount++;
            }

            // maybe too much if additives change frequently
            if (additives.Count > 0)
            {
                new NotificationService().ShowSimplePushNotification(1, additivesCount + " Additive(s) available", "Update Additives", 2, string.Empty);
            }
        }

    }

}