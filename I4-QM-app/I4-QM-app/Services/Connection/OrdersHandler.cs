using I4_QM_app.Models;
using MQTTnet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace I4_QM_app.Services.Connection
{
    /// <summary>
    /// Implementation of IMessageHandler for Orders.
    /// </summary>
    public class OrdersHandler : IMessageHandler
    {
        /// <summary>
        /// Handles add topic/route.
        /// </summary>
        /// <param name="message">Mqtt message.</param>
        /// <returns>Task.</returns>
        public async Task HandleAddRoute(MqttApplicationMessage message)
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
                    order.Status = Status.Open;
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
                        additive.ImageBase64 = item.ImageBase64;
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
                App.NotificationService.ShowSimplePushNotification(1, orderCount + " new order(s)", "New Order", 1, "OrdersPage");
            }
        }

        /// <summary>
        /// Handles delete topic/route.
        /// </summary>
        /// <param name="message">Mqtt message.</param>
        /// <returns>Task.</returns>
        public async Task HandleDelRoute(MqttApplicationMessage message)
        {
            string delOrders = Encoding.UTF8.GetString(message.Payload);

            Console.WriteLine($"+ Delete");

            bool parsable = int.TryParse(delOrders, out int status) && Enum.IsDefined(typeof(Status), status);

            if (parsable)
            {
                var orders = await App.OrdersDataService.GetItemsFilteredAsync(x => (int)x.Status == status);

                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                };

                string ordersString = System.Text.Json.JsonSerializer.Serialize(orders, options);
                await App.ConnectionService.HandlePublishMessage("backup/orders/" + ((Status)status).ToString(), ordersString);
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

        /// <summary>
        /// Handles get topic/route.
        /// </summary>
        /// <param name="message">Mqtt message.</param>
        /// <returns>Task.</returns>
        public async Task HandleGetRoute(MqttApplicationMessage message)
        {
            var getOrders = await App.OrdersDataService.GetItemsAsync();
            string ordersList = JsonConvert.SerializeObject(getOrders);
            await App.ConnectionService.HandlePublishMessage("backup/orders", ordersList);
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
            if (topic == baseTopicURL + "prod/orders/add")
            {
                await HandleAddRoute(message);
            }

            if (topic == baseTopicURL + "prod/orders/del")
            {
                await HandleDelRoute(message);
            }

            if (topic == baseTopicURL + "prod/orders/get")
            {
                await HandleGetRoute(message);
            }

            if (topic == baseTopicURL + "prod/orders/sync")
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
            await App.OrdersDataService.DeleteAllItemsAsync();
            await HandleAddRoute(message);
        }
    }
}
