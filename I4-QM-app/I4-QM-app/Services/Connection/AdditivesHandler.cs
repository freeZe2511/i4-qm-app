using I4_QM_app.Models;
using MQTTnet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace I4_QM_app.Services.Connection
{
    public class AdditivesHandler : IMessageHandler
    {
        public async Task HandleAddRoute(MqttApplicationMessage message)
        {
            Console.WriteLine($"+ Add Additives");

            string addAdditives = Encoding.UTF8.GetString(message.Payload);
            List<Additive> additives = JsonConvert.DeserializeObject<List<Additive>>(addAdditives);
            int additivesCount = 0;

            foreach (var additive in additives)
            {
                if (additive.Id == null || additive.Name == null)
                {
                    continue;
                }

                additive.ActualPortion = 0;
                additive.Amount = 0;
                additive.Portion = 0;
                additive.Checked = false;

                var fs = App.DB.GetStorage<string>("myImages");

                if (!String.IsNullOrEmpty(additive.ImageBase64))
                {
                    try
                    {
                        fs.Upload(additive.Id, additive.Name, new MemoryStream(Convert.FromBase64String(additive.ImageBase64)));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed to save image");
                    }

                }

                additive.ImageBase64 = null;

                await App.AdditivesDataService.AddItemAsync(additive);
                additivesCount++;
            }

            if (additives.Count > 0)
            {
                App.NotificationService.ShowSimplePushNotification(1, additivesCount + " Additive(s) added", "Update Additives", 2, string.Empty);
            }

        }

        public async Task HandleDelRoute(MqttApplicationMessage message)
        {
            string delAdditives = Encoding.UTF8.GetString(message.Payload);

            if (delAdditives.Equals("0"))
            {
                await App.AdditivesDataService.DeleteAllItemsAsync();
            }
            else
            {
                List<string> ids = JsonConvert.DeserializeObject<List<string>>(delAdditives);

                foreach (string id in ids)
                {
                    await App.AdditivesDataService.DeleteItemAsync(id);
                    var fs = App.DB.GetStorage<string>("myImages");
                    fs.Delete(id);
                }
            }
        }

        public async Task HandleGetRoute(MqttApplicationMessage message)
        {
            var getAdditives = await App.AdditivesDataService.GetItemsAsync();

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            };

            string additivesList = System.Text.Json.JsonSerializer.Serialize(getAdditives, options);
            await App.ConnectionService.HandlePublishMessage("backup/additives", additivesList);
        }

        public async Task HandleRoutes(MqttApplicationMessage message, string baseTopicURL)
        {
            var topic = message.Topic;

            // maybe not ideal
            if (topic == baseTopicURL + "prod/additives/add")
            {
                await HandleAddRoute(message);
            }

            if (topic == baseTopicURL + "prod/additives/del")
            {
                await HandleDelRoute(message);
            }

            if (topic == baseTopicURL + "prod/additives/get")
            {
                await HandleGetRoute(message);
            }

            if (topic == baseTopicURL + "prod/additives/sync")
            {
                await HandleUpdateRoute(message);
            }
        }

        public async Task HandleUpdateRoute(MqttApplicationMessage message)
        {
            await App.AdditivesDataService.DeleteAllItemsAsync();
            await HandleAddRoute(message);
        }

    }
}
