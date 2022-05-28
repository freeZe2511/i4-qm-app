using MQTTnet;
using MQTTnet.Client.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    public class Connection
    {

        public static async Task Send_Message()
        {
            /*
        * This sample pushes a simple application message including a topic and a payload.
        *
        * Always use builders where they exist. Builders (in this project) are designed to be
        * backward compatible. Creating an _MqttApplicationMessage_ via its constructor is also
        * supported but the class might change often in future releases where the builder does not
        * or at least provides backward compatibility where possible.
        */

            var mqttFactory = new MqttFactory();

            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("broker.hivemq.com")
                    .Build();

                await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

                var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("wow123")
                    .WithPayload("19.5")
                    .Build();

                await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

                Console.WriteLine("MQTT application message is published.");
            }
        }


    }
}
