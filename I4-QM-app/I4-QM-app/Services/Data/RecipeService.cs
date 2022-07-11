using I4_QM_app.Models;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.Services.Data
{
    /// <summary>
    /// Implementation of IDataService for Recipes with LiteDB.
    /// </summary>
    public class RecipeService : IDataService<Recipe>
    {
        private readonly ILiteCollection<Recipe> recipesCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeService"/> class.
        /// </summary>
        public RecipeService()
        {
            recipesCollection = App.DB.GetCollection<Recipe>("recipes");
        }

        /// <summary>
        /// Add recipe item to data store.
        /// </summary>
        /// <param name="recipe">Recipe.</param>
        /// <returns>Task.</returns>
        public async Task<bool> AddItemAsync(Recipe recipe)
        {
            recipesCollection.Insert(recipe);
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Update recipe item from data store.
        /// </summary>
        /// <param name="recipe">Recipe.</param>
        /// <returns>Task.</returns>
        public async Task<bool> UpdateItemAsync(Recipe recipe)
        {
            recipesCollection.Update(recipe);
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Delete recipe item from data store with id.
        /// </summary>
        /// <param name="id">Unique id.</param>
        /// <returns>Task.</returns>
        public async Task<bool> DeleteItemAsync(string id)
        {
            recipesCollection.Delete(id);
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Delete many recipe items from data store with predicate.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Task.</returns>
        public async Task<bool> DeleteManyItemsAsync(System.Func<Recipe, bool> predicate)
        {
            // TODO Func predicate -> BsonExpression ???
            var list = recipesCollection.FindAll().Where(predicate).ToList();
            list.ForEach(i => recipesCollection.Delete(i.Id));
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Delete all recipe items from data store.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task<bool> DeleteAllItemsAsync()
        {
            recipesCollection.DeleteAll();
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Get recipe item from data store with id.
        /// </summary>
        /// <param name="id">Unique id.</param>
        /// <returns>Task.</returns>
        public async Task<Recipe> GetItemAsync(string id)
        {
            var recipe = recipesCollection.FindAll().FirstOrDefault(a => a.Id == id);
            return await Task.FromResult(recipe);
        }

        /// <summary>
        /// Get recipe items from data store.
        /// </summary>
        /// <returns>Task.</returns
        public async Task<IEnumerable<Recipe>> GetItemsAsync()
        {
            return await Task.FromResult(recipesCollection.FindAll());
        }

        /// <summary>
        /// Get recipe items from data store with predicate.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Task.</returns>
        public async Task<IEnumerable<Recipe>> GetItemsFilteredAsync(System.Func<Recipe, bool> predicate)
        {
            return await Task.FromResult(recipesCollection.FindAll().Where(predicate).ToList());
        }
    }
}
