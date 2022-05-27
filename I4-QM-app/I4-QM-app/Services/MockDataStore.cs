using I4_QM_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    public class MockDataStore : IDataStore<Order>
    {
        readonly List<Order> orders;

        public MockDataStore()
        {
            orders = new List<Order>()
            {
                new Order {Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="1", Amount= 10, Name="Rot"},
                    new Additive {Id="2", Amount= 90, Name="UV"} }, Amount = 100, Status = Status.test, Created= new DateTime(2022, 05, 25), Due= new DateTime(2022, 06, 01)},
                new Order {Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="3", Amount= 50, Name="Blau"},
                    new Additive {Id="4", Amount= 50, Name="G"} }, Amount = 200, Status = Status.waiting, Created= new DateTime(2022, 05, 26), Due= new DateTime(2022, 06, 02)},
                new Order {Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="1", Amount= 10, Name="Rot"},
                    new Additive {Id="2", Amount= 90, Name="UV"} }, Amount = 100, Status = Status.test, Created= new DateTime(2022, 05, 25), Due= new DateTime(2022, 06, 01)},
                new Order {Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="1", Amount= 10, Name="Rot"},
                    new Additive {Id="2", Amount= 90, Name="UV"} }, Amount = 100, Status = Status.test, Created= new DateTime(2022, 05, 25), Due= new DateTime(2022, 06, 01)},
                new Order {Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="1", Amount= 10, Name="Rot"},
                    new Additive {Id="2", Amount= 90, Name="UV"} }, Amount = 100, Status = Status.test, Created= new DateTime(2022, 05, 25), Due= new DateTime(2022, 06, 01)},
                new Order {Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="1", Amount= 10, Name="Rot"},
                    new Additive {Id="2", Amount= 90, Name="UV"} }, Amount = 100, Status = Status.test, Created= new DateTime(2022, 05, 25), Due= new DateTime(2022, 06, 01)},
                new Order {Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="1", Amount= 10, Name="Rot"},
                    new Additive {Id="2", Amount= 90, Name="UV"} }, Amount = 100, Status = Status.test, Created= new DateTime(2022, 05, 25), Due= new DateTime(2022, 06, 01)},
                new Order {Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="1", Amount= 10, Name="Rot"},
                    new Additive {Id="2", Amount= 90, Name="UV"} }, Amount = 100, Status = Status.test, Created= new DateTime(2022, 05, 25), Due= new DateTime(2022, 06, 01)},
                new Order {Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="1", Amount= 10, Name="Rot"},
                    new Additive {Id="2", Amount= 90, Name="UV"} }, Amount = 100, Status = Status.test, Created= new DateTime(2022, 05, 25), Due= new DateTime(2022, 06, 01)},
                new Order {Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="1", Amount= 10, Name="Rot"},
                    new Additive {Id="2", Amount= 90, Name="UV"} }, Amount = 100, Status = Status.test, Created= new DateTime(2022, 05, 25), Due= new DateTime(2022, 06, 01)},
                new Order {Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="1", Amount= 10, Name="Rot"},
                    new Additive {Id="2", Amount= 90, Name="UV"} }, Amount = 100, Status = Status.test, Created= new DateTime(2022, 05, 25), Due= new DateTime(2022, 06, 01)},
                new Order {Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="1", Amount= 10, Name="Rot"},
                    new Additive {Id="2", Amount= 90, Name="UV"} }, Amount = 100, Status = Status.test, Created= new DateTime(2022, 05, 25), Due= new DateTime(2022, 06, 01)},
                new Order {Id = Guid.NewGuid().ToString(), UserId = Guid.NewGuid().ToString(), Additives = new List<Additive>{ new Additive {Id="1", Amount= 10, Name="Rot"},
                    new Additive {Id="2", Amount= 90, Name="UV"} }, Amount = 100, Status = Status.test, Created= new DateTime(2022, 05, 25), Due= new DateTime(2022, 06, 01)},
            };
        }

        public async Task<bool> AddOrderAsync(Order order)
        {
            orders.Add(order);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            var oldOrder = orders.Where((Order arg) => arg.Id == order.Id).FirstOrDefault();
            orders.Remove(oldOrder);
            orders.Add(order);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteOrderAsync(string id)
        {
            var oldOrder = orders.Where((Order arg) => arg.Id == id).FirstOrDefault();
            orders.Remove(oldOrder);

            return await Task.FromResult(true);
        }

        public async Task<Order> GetOrderAsync(string id)
        {
            return await Task.FromResult(orders.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(orders);
        }
    }
}