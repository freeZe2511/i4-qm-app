using I4_QM_app.Models;
using MQTTnet;
using MQTTnet.Client.Options;
using System.Threading;
using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    public class MqttConnection
    {

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

        // TODO subscribe to messages -> use content to ... ?


    }
}
