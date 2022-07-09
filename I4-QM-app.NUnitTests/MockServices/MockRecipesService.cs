using I4_QM_app.Models;
using I4_QM_app.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.MockServices
{
    public class MockRecipesService : IDataService<Recipe>
    {
        public Task<bool> AddItemAsync(Recipe item)
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

        public Task<bool> DeleteManyItemsAsync(Func<Recipe, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Recipe> GetItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Recipe>> GetItemsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Recipe>> GetItemsFilteredAsync(Func<Recipe, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(Recipe item)
        {
            throw new NotImplementedException();
        }
    }
}
