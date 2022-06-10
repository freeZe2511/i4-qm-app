using I4_QM_app.Models;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.Helpers
{
    public class OrderService : IDataStore<Order>
    {
        private readonly ILiteCollection<Order> orderCollection;

        public OrderService()
        {
            orderCollection = App.DB.GetCollection<Order>("orders");
            //orderCollection.EnsureIndex(x => x.Id);
        }

        public async Task<bool> AddItemAsync(Order order)
        {
            //if (orderCollection.Exists(order.Id)) 
            orderCollection.Insert(order);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Order order)
        {
            orderCollection.Update(order);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            // index?
            orderCollection.Delete(id);
            return await Task.FromResult(true);
        }

        public async Task<Order> GetItemAsync(string id)
        {
            var order = orderCollection.FindAll().Where(a => a.Id == id).FirstOrDefault();
            return await Task.FromResult(order);
        }

        public async Task<IEnumerable<Order>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(orderCollection.FindAll());
        }

        public async Task<IEnumerable<Order>> GetItemsFilteredAsync(System.Func<Order, bool> predicate)
        {
            return await Task.FromResult(orderCollection.FindAll().Where(predicate).ToList());
        }

    }
}
