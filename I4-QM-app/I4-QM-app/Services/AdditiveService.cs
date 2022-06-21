using I4_QM_app.Models;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    public class AdditiveService : IDataStore<Additive>
    {
        private readonly ILiteCollection<Additive> additivesCollection;
        public AdditiveService()
        {
            additivesCollection = App.DB.GetCollection<Additive>("additives");
        }

        public async Task<bool> AddItemAsync(Additive additive)
        {
            additivesCollection.Insert(additive);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Additive additive)
        {
            additivesCollection.Update(additive);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            additivesCollection.Delete(id);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteManyItemsAsync(System.Func<Additive, bool> predicate)
        {
            // TODO Func predicate -> BsonExpression ???
            var list = additivesCollection.FindAll().Where(predicate).ToList();
            list.ForEach(i => additivesCollection.Delete(i.Id));
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAllItemsAsync()
        {
            additivesCollection.DeleteAll();
            return await Task.FromResult(true);
        }

        public async Task<Additive> GetItemAsync(string id)
        {
            var additive = additivesCollection.FindAll().Where(a => a.Id == id).FirstOrDefault();
            return await Task.FromResult(additive);
        }

        public async Task<IEnumerable<Additive>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(additivesCollection.FindAll());
        }

        public async Task<IEnumerable<Additive>> GetItemsFilteredAsync(System.Func<Additive, bool> predicate)
        {
            return await Task.FromResult(additivesCollection.FindAll().Where(predicate).ToList());
        }
    }
}
