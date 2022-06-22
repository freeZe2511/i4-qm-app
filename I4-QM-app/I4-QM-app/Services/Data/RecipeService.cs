using I4_QM_app.Models;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.Services
{
    public class RecipeService : IDataService<Recipe>
    {
        private readonly ILiteCollection<Recipe> recipesCollection;
        public RecipeService()
        {
            recipesCollection = App.DB.GetCollection<Recipe>("recipes");
        }

        public async Task<bool> AddItemAsync(Recipe recipe)
        {
            recipesCollection.Insert(recipe);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Recipe recipe)
        {
            recipesCollection.Update(recipe);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            recipesCollection.Delete(id);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteManyItemsAsync(System.Func<Recipe, bool> predicate)
        {
            // TODO Func predicate -> BsonExpression ???
            var list = recipesCollection.FindAll().Where(predicate).ToList();
            list.ForEach(i => recipesCollection.Delete(i.Id));
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAllItemsAsync()
        {
            recipesCollection.DeleteAll();
            return await Task.FromResult(true);
        }

        public async Task<Recipe> GetItemAsync(string id)
        {
            var recipe = recipesCollection.FindAll().Where(a => a.Id == id).FirstOrDefault();
            return await Task.FromResult(recipe);
        }

        public async Task<IEnumerable<Recipe>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(recipesCollection.FindAll());
        }

        public async Task<IEnumerable<Recipe>> GetItemsFilteredAsync(System.Func<Recipe, bool> predicate)
        {
            return await Task.FromResult(recipesCollection.FindAll().Where(predicate).ToList());
        }
    }
}
