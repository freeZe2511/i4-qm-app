using I4_QM_app.Models;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    public class MqttConnection
    {

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

                var applicationMessage = new MqttApplicationMessageBuilder()
                    // TODO mqtt topic+message concept
                    .WithTopic("sfm/sg/ready")
                    .WithPayload(item.Id)
                    .Build();

                await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

            }
        }

        public static async Task Handle_Received_Application_Message()
        {
            /*
             * This sample subscribes to a topic and processes the received message.
             */

            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("broker.hivemq.com")
                    .Build();

                // Setup message handling before connecting so that queued messages
                // are also handled properly.When there is no event handler attached all
                //received messages get lost.
                //mqttClient.ApplicationMessageReceivedHandler += e =>
                // {
                //     Console.WriteLine("Received application message.");
                //     e.DumpToConsole();

                //     return Task.CompletedTask;
                // };

                //mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(obj =>
                //            {
                //                Console.WriteLine(obj);
                //            });

                Console.WriteLine("test");

                mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
                {
                    Console.WriteLine("Received application message.");
                    Console.WriteLine(e);
                });




                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(f => { f.WithTopic("sfm/sg/order"); })
                    .Build();

                await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            }
        }

        private void HandleMessageReceived(MqttApplicationMessage applicationMessage)
        {
            Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
            Console.WriteLine($"+ Topic = {applicationMessage.Topic}");

            Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(applicationMessage.Payload)}");
            Console.WriteLine($"+ QoS = {applicationMessage.QualityOfServiceLevel}");
            Console.WriteLine($"+ Retain = {applicationMessage.Retain}");
            Console.WriteLine();
        }



        // TODO subscribe to messages -> use content to ... ?


    }
}
