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

namespace I4_QM_app.Services
{
    public class MqttConnection
    {
        private static IMqttClient _mqttClient;

        // reconnect auto?
        // https://github.com/dotnet/MQTTnet/blob/master/Samples/ManagedClient/Managed_Client_Simple_Samples.cs

        // refactor 1 connection
        public static async Task Send_Message(Order item)
        {
            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("broker.hivemq.com")
                    .Build();

                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                PublishMessage("sfm/sg/ready", item.Id);

            }
        }

        public static async Task Handle_Received_Application_Message()
        {
            // Create client
            if (_mqttClient == null) _mqttClient = new MqttFactory().CreateMqttClient();

            var options = new MqttClientOptionsBuilder().WithClientId("QM-App")
                                                        .WithTcpServer("broker.hivemq.com")
                                                        .Build();
            // When client connected to the server
            _mqttClient.UseConnectedHandler(async e =>
            {
                // Subscribe to a topic TODO topic filter
                MqttClientSubscribeResult subResult = await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                                                                   .WithTopicFilter("sfm/sg/#")
                                                                   .Build());
                // Sen a test message to the server
                PublishMessage("sfm/sg/connected", "QM App connected");
            });

            // When client received a message from server
            _mqttClient.UseApplicationMessageReceivedHandler(async e =>
            {
                // refactor 
                switch (e.ApplicationMessage.Topic)
                {
                    case "sfm/sg/order/add":
                        var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                        Console.WriteLine($"+ Add");

                        //serialize order and add to db
                        List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(message);

                        foreach (var order in orders)
                        {
                            await App.OrdersDataStore.AddItemAsync(order);
                        }
                        break;

                    case "sfm/sg/order/delete":
                        var id = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                        Console.WriteLine($"+ Delete = {id}");

                        //delete order with orderId
                        await App.OrdersDataStore.DeleteItemAsync(id);
                        break;

                    case "sfm/sg/order/all":
                        //delete order with orderId
                        var orders1 = await App.OrdersDataStore.GetItemsAsync();

                        foreach (var order in orders1)
                        {
                            PublishMessage("sfm/sg/xxx", order.Id);
                        }
                        break;

                    default:
                        Console.WriteLine($"+ Rest = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        break;
                }



            });

            // Connect ot server
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

    }




}

