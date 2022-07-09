using I4_QM_app.Models;
using I4_QM_app.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.MockServices
{
    public class MockAdditivesService : IDataService<Additive>
    {
        private readonly List<Additive> additives;

        public MockAdditivesService()
        {
            this.additives = new List<Additive>();
        }

        public async Task<bool> AddItemAsync(Additive item)
        {
            additives.Add(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAllItemsAsync()
        {
            additives.Clear();
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldAdditive = additives.Where((Additive arg) => arg.Id == id).FirstOrDefault();
            additives.Remove(oldAdditive);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteManyItemsAsync(Func<Additive, bool> predicate)
        {
            var list = additives.Where(predicate).ToList();
            list.ForEach(i => additives.Remove(i));
            return await Task.FromResult(true);
        }

        public async Task<Additive> GetItemAsync(string id)
        {
            return await Task.FromResult(additives.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Additive>> GetItemsAsync()
        {
            return await Task.FromResult(additives);
        }

        public async Task<IEnumerable<Additive>> GetItemsFilteredAsync(Func<Additive, bool> predicate)
        {
            return await Task.FromResult(additives.Where(predicate).ToList());
        }

        public async Task<bool> UpdateItemAsync(Additive item)
        {
            var oldAdditive = additives.Where((Additive arg) => arg.Id == item.Id).FirstOrDefault();
            additives.Remove(oldAdditive);
            additives.Add(item);
            return await Task.FromResult(true);
        }
    }
}
