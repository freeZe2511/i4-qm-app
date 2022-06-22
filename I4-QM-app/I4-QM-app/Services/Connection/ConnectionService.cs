using I4_QM_app.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    public class ConnectionService : IConnectionService
    {
        private static string serverURL = "broker.hivemq.com";
        private static string baseTopicURL = "thm/sfm/sg/";
        private IManagedMqttClient managedMqttClient;

        public ConnectionService()
        {
            managedMqttClient = new MqttFactory().CreateManagedMqttClient();
            Task.Run(async () => await ConnectClient());
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

            // maybe not ideal
            if (topic == baseTopicURL + "prod/orders/add") await HandleAddOrder(message);
            if (topic == baseTopicURL + "prod/orders/del") await HandleDelOrder(message);
            if (topic == baseTopicURL + "prod/orders/get") await HandleGetOrder(message);
            if (topic == baseTopicURL + "additives/sync") await HandleSyncAdditives(message);

            return Task.CompletedTask;
        }

        public async Task HandlePublishMessage(string topic, string message)
        {
            await managedMqttClient.EnqueueAsync(baseTopicURL + topic, message, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
        }

        private async Task HandleAddOrder(MqttApplicationMessage message)
        {
            string addOrders = Encoding.UTF8.GetString(message.Payload);

            Console.WriteLine($"+ Add");

            //serialize order and add to db
            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(addOrders);

            int orderCount = 0;

            foreach (var order in orders)
            {
                //check if id is unique
                if (await App.OrdersDataStore.GetItemAsync(order.Id) == null)
                {
                    order.Status = Status.open;
                    await App.OrdersDataStore.AddItemAsync(order);
                    orderCount++;
                }
            }

            //notification TODO service
            if (orderCount > 0) new NotificationService().ShowSimplePushNotification(1, orderCount + " new order(s)", "New Order", 1, "OrdersPage");

        }

        private async Task HandleDelOrder(MqttApplicationMessage message)
        {
            string delOrders = Encoding.UTF8.GetString(message.Payload);

            Console.WriteLine($"+ Delete");

            bool parsable = int.TryParse(delOrders, out int status) && Enum.IsDefined(typeof(Status), status);

            if (parsable)
            {
                var orders = await App.OrdersDataStore.GetItemsFilteredAsync(x => (int)x.Status == status);

                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                };

                string ordersString = System.Text.Json.JsonSerializer.Serialize(orders, options);

                //string ordersString = JsonConvert.SerializeObject(orders);
                await HandlePublishMessage("backup/orders/" + ((Status)status).ToString(), ordersString);

                await App.OrdersDataStore.DeleteManyItemsAsync(x => (int)x.Status == status);
                return;
            }
            else
            {
                List<string> ids = JsonConvert.DeserializeObject<List<string>>(delOrders);

                foreach (string id in ids)
                {
                    await App.OrdersDataStore.DeleteItemAsync(id);
                }
            }

        }

        private async Task HandleGetOrder(MqttApplicationMessage message)
        {
            var getOrders = await App.OrdersDataStore.GetItemsAsync();

            string ordersList = JsonConvert.SerializeObject(getOrders);

            await HandlePublishMessage("backup/orders", ordersList);
        }

        private async Task HandleSyncAdditives(MqttApplicationMessage message)
        {
            string req = Encoding.UTF8.GetString(message.Payload);

            Console.WriteLine($"+ Sync Additives");

            List<Additive> additives = JsonConvert.DeserializeObject<List<Additive>>(req);

            await App.AdditivesDataStore.DeleteAllItemsAsync();

            foreach (Additive additive in additives)
            {
                additive.ActualPortion = 0;
                additive.Amount = 0;
                additive.Portion = 0;
                additive.Checked = false;

                await App.AdditivesDataStore.AddItemAsync(additive);
            }

            // maybe too much if additives change frequently
            if (additives.Count > 0) new NotificationService().ShowSimplePushNotification(1, additives.Count + " Additive(s) available", "Update Additives", 2, "");
        }

    }

}

