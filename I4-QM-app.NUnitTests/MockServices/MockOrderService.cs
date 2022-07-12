using I4_QM_app.Models;
using I4_QM_app.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.MockServices
{
    internal class MockOrderService : IDataService<Order>
    {
        private readonly List<Order> orders;

        public MockOrderService()
        {
            this.orders = new List<Order>();
        }

        public async Task<bool> AddItemAsync(Order item)
        {
            orders.Add(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAllItemsAsync()
        {
            orders.Clear();
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldOrder = orders.Where((Order arg) => arg.Id == id).FirstOrDefault();
            orders.Remove(oldOrder);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteManyItemsAsync(Func<Order, bool> predicate)
        {
            var list = orders.Where(predicate).ToList();
            list.ForEach(i => orders.Remove(i));
            return await Task.FromResult(true);
        }

        public async Task<Order> GetItemAsync(string id)
        {
            return await Task.FromResult(orders.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Order>> GetItemsAsync()
        {
            return await Task.FromResult(orders);
        }

        public async Task<IEnumerable<Order>> GetItemsFilteredAsync(Func<Order, bool> predicate)
        {
            return await Task.FromResult(orders.Where(predicate).ToList());
        }

        public async Task<bool> UpdateItemAsync(Order item)
        {
            var oldOrder = orders.Where((Order arg) => arg.Id == item.Id).FirstOrDefault();
            orders.Remove(oldOrder);
            orders.Add(item);
            return await Task.FromResult(true);
        }
    }
}
