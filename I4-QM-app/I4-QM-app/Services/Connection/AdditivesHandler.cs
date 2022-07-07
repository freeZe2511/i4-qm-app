using I4_QM_app.Models;
using LiteDB;
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
    /// <summary>
    /// Implementation of IMessageHandler for Additives.
    /// </summary>
    public class AdditivesHandler : IMessageHandler
    {
        /// <summary>
        /// Handles add topic/route.
        /// </summary>
        /// <param name="message">Mqtt message.</param>
        /// <returns>Task.</returns>
        public async Task HandleAddRoute(MqttApplicationMessage message)
        {
            Console.WriteLine($"+ Add Additives");

            string addAdditives = Encoding.UTF8.GetString(message.Payload);
            List<Additive> additives = JsonConvert.DeserializeObject<List<Additive>>(addAdditives);
            int additivesCount = 0;
            var fs = App.DB.GetStorage<string>("myImages");

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

                TryUploadAdditiveImage(fs, additive);
                additive.ImageBase64 = null;

                await App.AdditivesDataService.AddItemAsync(additive);
                additivesCount++;
            }

            if (additives.Count > 0)
            {
                App.NotificationService.ShowSimplePushNotification(1, additivesCount + " Additive(s) added", "Update Additives", 2, string.Empty);
            }
        }

        /// <summary>
        /// Handles delete topic/route.
        /// </summary>
        /// <param name="message">Mqtt message.</param>
        /// <returns>Task.</returns>
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

        /// <summary>
        /// Handles get topic/route.
        /// </summary>
        /// <param name="message">Mqtt message.</param>
        /// <returns>Task.</returns>
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

        /// <summary>
        /// Handler to redirect to specific method per route.
        /// </summary>
        /// <param name="message">Mqtt message.</param>
        /// <param name="baseTopicURL">mqtt base url.</param>
        /// <returns>Task.</returns>
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

        /// <summary>
        /// Handles update topic/route.
        /// </summary>
        /// <param name="message">Mqtt message.</param>
        /// <returns>Task.</returns>
        public async Task HandleUpdateRoute(MqttApplicationMessage message)
        {
            await App.AdditivesDataService.DeleteAllItemsAsync();
            await HandleAddRoute(message);
        }

        /// <summary>
        /// Additives image upload handler.
        /// </summary>
        /// <param name="fs">Filestorage.</param>
        /// <param name="additive">Additive.</param>
        private void TryUploadAdditiveImage(ILiteStorage<string> fs, Additive additive)
        {
            if (!string.IsNullOrEmpty(additive.ImageBase64))
            {
                try
                {
                    fs.Upload(additive.Id, additive.Name, new MemoryStream(Convert.FromBase64String(additive.ImageBase64)));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to save image");
                    Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
