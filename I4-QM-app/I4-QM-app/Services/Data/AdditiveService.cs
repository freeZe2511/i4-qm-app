using I4_QM_app.Models;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    /// <summary>
    /// Implementation of IDataService for Additives with LiteDB.
    /// </summary>
    public class AdditiveService : IDataService<Additive>
    {
        private readonly ILiteCollection<Additive> additivesCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditiveService"/> class.
        /// </summary>
        public AdditiveService()
        {
            additivesCollection = App.DB.GetCollection<Additive>("additives");
        }

        /// <summary>
        /// Add additive item to data store.
        /// </summary>
        /// <param name="additive">Additive.</param>
        /// <returns>Task.</returns>
        public async Task<bool> AddItemAsync(Additive additive)
        {
            additivesCollection.Insert(additive);
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Update additive item from data store.
        /// </summary>
        /// <param name="additive">Additive.</param>
        /// <returns>Task.</returns>
        public async Task<bool> UpdateItemAsync(Additive additive)
        {
            additivesCollection.Update(additive);
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Delete additive item from data store with id.
        /// </summary>
        /// <param name="id">Unique id.</param>
        /// <returns>Task.</returns>
        public async Task<bool> DeleteItemAsync(string id)
        {
            additivesCollection.Delete(id);
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Delete many additive items from data store with predicate.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Task.</returns>
        public async Task<bool> DeleteManyItemsAsync(System.Func<Additive, bool> predicate)
        {
            // TODO Func predicate -> BsonExpression ???
            var list = additivesCollection.FindAll().Where(predicate).ToList();
            list.ForEach(i => additivesCollection.Delete(i.Id));
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Delete all additive items from data store.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task<bool> DeleteAllItemsAsync()
        {
            additivesCollection.DeleteAll();
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Get additive item from data store with id.
        /// </summary>
        /// <param name="id">Unique id.</param>
        /// <returns>Task.</returns>
        public async Task<Additive> GetItemAsync(string id)
        {
            var additive = additivesCollection.FindAll().FirstOrDefault(a => a.Id == id);
            return await Task.FromResult(additive);
        }

        /// <summary>
        /// Get additive items from data store.
        /// </summary>
        /// <returns>Task.</returns
        public async Task<IEnumerable<Additive>> GetItemsAsync()
        {
            return await Task.FromResult(additivesCollection.FindAll());
        }

        /// <summary>
        /// Get additive items from data store with predicate.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Task.</returns>
        public async Task<IEnumerable<Additive>> GetItemsFilteredAsync(System.Func<Additive, bool> predicate)
        {
            return await Task.FromResult(additivesCollection.FindAll().Where(predicate).ToList());
        }
    }
}
