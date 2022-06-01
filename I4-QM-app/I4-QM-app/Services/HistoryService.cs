using I4_QM_app.Models;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    public class HistoryService : IDataStore<Order>
    {
        private readonly ILiteCollection<Order> historyCollection;

        public HistoryService()
        {
            historyCollection = App.DB.GetCollection<Order>("history");
            Console.WriteLine("hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh");
            Console.WriteLine(historyCollection);
            //orderCollection.EnsureIndex(x => x.Id);
        }

        public async Task<bool> AddItemAsync(Order order)
        {
            //if (orderCollection.Exists(order.Id)) 
            historyCollection.Insert(order);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Order order)
        {
            historyCollection.Update(order);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            // index?
            historyCollection.Delete(id);
            return await Task.FromResult(true);
        }

        public async Task<Order> GetItemAsync(string id)
        {
            var order = historyCollection.FindAll().Where(a => a.Id == id).FirstOrDefault();
            return await Task.FromResult(order);
        }

        public async Task<IEnumerable<Order>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(historyCollection.FindAll());
        }
    }
}
