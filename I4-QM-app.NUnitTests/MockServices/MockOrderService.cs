using I4_QM_app.Models;
using I4_QM_app.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.MockServices
{
    internal class MockOrderService : IDataService<Order>
    {
        public Task<bool> AddItemAsync(Order item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAllItemsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteManyItemsAsync(Func<Order, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetItemsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Order>> GetItemsFilteredAsync(Func<Order, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(Order item)
        {
            throw new NotImplementedException();
        }
    }
}
