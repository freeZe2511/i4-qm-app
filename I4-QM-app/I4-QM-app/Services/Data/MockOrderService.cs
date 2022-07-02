﻿using I4_QM_app.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    public class MockOrderService : IDataService<Order>
    {
        readonly List<Order> orders;

        public MockOrderService()
        {
            orders = new List<Order>()
            {
                //new Order {Id = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="1", Amount= 10, Name="Rot"},
                //    new Additive {Id="2", Amount= 90, Name="UV"} }, Weight = 5, Amount = 100, Created= new DateTime(2022, 05, 25), Due= new DateTime(2022, 08, 01)},

            };
        }

        public async Task<bool> AddItemAsync(Order order)
        {
            orders.Add(order);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Order order)
        {
            var oldOrder = orders.Where((Order arg) => arg.Id == order.Id).FirstOrDefault();
            orders.Remove(oldOrder);
            orders.Add(order);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldOrder = orders.Where((Order arg) => arg.Id == id).FirstOrDefault();
            orders.Remove(oldOrder);

            return await Task.FromResult(true);
        }

        public async Task<Order> GetItemAsync(string id)
        {
            return await Task.FromResult(orders.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Order>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(orders);
        }

        public async Task<IEnumerable<Order>> GetItemsFilteredAsync(System.Func<Order, bool> predicate)
        {
            return await Task.FromResult(orders.Where(predicate).ToList());
        }

        public async Task<bool> DeleteAllItemsAsync()
        {
            orders.Clear();
            return await Task.FromResult(true);
        }

        public Task<bool> DeleteManyItemsAsync(System.Func<Order, bool> predicate)
        {
            throw new System.NotImplementedException();
        }
    }
}