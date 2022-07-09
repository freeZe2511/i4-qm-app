using I4_QM_app.Models;
using I4_QM_app.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.MockServices
{
    public class MockAdditivesService : IDataService<Additive>
    {
        public Task<bool> AddItemAsync(Additive item)
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

        public Task<bool> DeleteManyItemsAsync(Func<Additive, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Additive> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Additive>> GetItemsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Additive>> GetItemsFilteredAsync(Func<Additive, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(Additive item)
        {
            throw new NotImplementedException();
        }
    }
}
