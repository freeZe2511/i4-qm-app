using I4_QM_app.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Packets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (topic == baseTopicURL + "prod/orders/get") await HandleGetOrder();
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

            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(addOrders);
            int orderCount = 0;

            foreach (var order in orders)
            {
                bool error = false;

                if (order.Id == null)
                {
                    order.Id = Guid.NewGuid().ToString();
                }

                if (order.Weight > 0 && order.Amount > 0 && order.Additives.Count > 0 && order.Due > DateTime.Now && await App.OrdersDataService.GetItemAsync(order.Id) == null)
                {
                    order.Status = Status.open;
                    order.Received = DateTime.Now;

                    var additives = await App.AdditivesDataService.GetItemsAsync();

                    foreach (var additive in order.Additives)
                    {
                        if (additive.Id == null || additive.Portion <= 0)
                        {
                            error = true;
                            break;
                        }

                        Additive item = additives.FirstOrDefault(x => x.Id == additive.Id);

                        if (item == null)
                        {
                            error = true;
                            break;
                        }

                        additive.Name = item.Name;
                        //additive.ImageId = item.ImageId
                    }

                    if (!error)
                    {
                        await App.OrdersDataService.AddItemAsync(order);
                        orderCount++;
                    }
                }
            }

            if (orderCount > 0)
            {
                new NotificationService().ShowSimplePushNotification(1, orderCount + " new order(s)", "New Order", 1, "OrdersPage");
            }
        }

        private async Task HandleDelOrder(MqttApplicationMessage message)
        {
            string delOrders = Encoding.UTF8.GetString(message.Payload);

            Console.WriteLine($"+ Delete");

            bool parsable = int.TryParse(delOrders, out int status) && Enum.IsDefined(typeof(Status), status);

            if (parsable)
            {
                var orders = await App.OrdersDataService.GetItemsFilteredAsync(x => (int)x.Status == status);

                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                };

                string ordersString = System.Text.Json.JsonSerializer.Serialize(orders, options);
                await HandlePublishMessage("backup/orders/" + ((Status)status).ToString(), ordersString);
                await App.OrdersDataService.DeleteManyItemsAsync(x => (int)x.Status == status);
            }
            else
            {
                List<string> ids = JsonConvert.DeserializeObject<List<string>>(delOrders);

                foreach (string id in ids)
                {
                    await App.OrdersDataService.DeleteItemAsync(id);
                }
            }

        }

        private async Task HandleGetOrder()
        {
            var getOrders = await App.OrdersDataService.GetItemsAsync();
            string ordersList = JsonConvert.SerializeObject(getOrders);
            await HandlePublishMessage("backup/orders", ordersList);
        }

        private async Task HandleSyncAdditives(MqttApplicationMessage message)
        {
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

