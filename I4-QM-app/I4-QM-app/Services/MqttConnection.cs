using I4_QM_app.Models;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace I4_QM_app.Helpers
{
    public class MqttConnection
    {
        private static string serverURL = "broker.hivemq.com";
        private static string baseTopicURL = "sfm/sg/";
        private static IMqttClient _mqttClient;

        // reconnect auto?
        // https://github.com/dotnet/MQTTnet/blob/master/Samples/ManagedClient/Managed_Client_Simple_Samples.cs


        // refactor 1 connection -> connectionHandler
        public static async Task HandleFinishedOrder(Order item)
        {
            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(serverURL)
                    .Build();

                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                // serialize order to string
                var message = JsonConvert.SerializeObject(item);

                PublishMessage(baseTopicURL + "order/ready", message);

            }
        }

        public static async Task ConnectClient()
        {
            // Create client
            if (_mqttClient == null) _mqttClient = new MqttFactory().CreateMqttClient();

            var options = new MqttClientOptionsBuilder().WithClientId("QM-App")
                                                        .WithTcpServer(serverURL)
                                                        .Build();
            // When client connected to the server
            HandleInitialConnection();

            // When client received a message from server
            HandleReceivedMessage();

            // Connect to server
            await _mqttClient.ConnectAsync(options, CancellationToken.None);

        }

        private static async void PublishMessage(string topic, string message)
        {
            // Create mqttMessage
            var mqttMessage = new MqttApplicationMessageBuilder()
                                .WithTopic(topic)
                                .WithPayload(message)
                                .WithExactlyOnceQoS()
                                .Build();

            // Publish the message asynchronously
            await _mqttClient.PublishAsync(mqttMessage, CancellationToken.None);
        }

        private static void HandleInitialConnection()
        {
            _mqttClient.UseConnectedHandler(async e =>
            {
                // Subscribe to a topic TODO topic filter
                MqttClientSubscribeResult subResult = await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                                                                   .WithTopicFilter(baseTopicURL + "#")
                                                                   .Build());
                // Sen a test message to the server
                PublishMessage(baseTopicURL + "connected", "QM App connected");
            });
        }

        private static void HandleReceivedMessage()
        {
            _mqttClient.UseApplicationMessageReceivedHandler(async e =>
            {
                // refactor 
                // push notification when smth happens?
                switch (e.ApplicationMessage.Topic)
                {
                    case "sfm/sg/order/add":
                        var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                        Console.WriteLine($"+ Add");

                        //serialize order and add to db
                        List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(message);

                        foreach (var order in orders)
                        {
                            order.Status = Status.open;
                            await App.OrdersDataStore.AddItemAsync(order);
                        }
                        break;

                    case "sfm/sg/order/del":
                        // maybe refactor to be able to delete from id list
                        var id = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                        Console.WriteLine($"+ Delete = {id}");

                        //delete order with orderId
                        await App.OrdersDataStore.DeleteItemAsync(id);
                        break;

                    case "sfm/sg/order/get":
                        //delete order with orderId
                        var orders1 = await App.OrdersDataStore.GetItemsAsync();

                        // todo send list, not single
                        foreach (var order in orders1)
                        {
                            PublishMessage("sfm/sg/order/all", order.Id);
                        }
                        break;

                    default:
                        Console.WriteLine($"+ Rest = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        break;
                }



            });
        }

    }




}

