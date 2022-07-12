using I4_QM_app.Models;
using I4_QM_app.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.MockServices
{
    public class MockRecipesService : IDataService<Recipe>
    {
        private readonly List<Recipe> recipes;

        public MockRecipesService()
        {
            this.recipes = new List<Recipe>();
        }

        public async Task<bool> AddItemAsync(Recipe item)
        {
            recipes.Add(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAllItemsAsync()
        {
            recipes.Clear();
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldRecipe = recipes.Where((Recipe arg) => arg.Id == id).FirstOrDefault();
            recipes.Remove(oldRecipe);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteManyItemsAsync(Func<Recipe, bool> predicate)
        {
            var list = recipes.Where(predicate).ToList();
            list.ForEach(i => recipes.Remove(i));
            return await Task.FromResult(true);
        }

        public async Task<Recipe> GetItemAsync(string id)
        {
            return await Task.FromResult(recipes.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Recipe>> GetItemsAsync()
        {
            return await Task.FromResult(recipes);
        }

        public async Task<IEnumerable<Recipe>> GetItemsFilteredAsync(Func<Recipe, bool> predicate)
        {
            return await Task.FromResult(recipes.Where(predicate).ToList());
        }

        public async Task<bool> UpdateItemAsync(Recipe item)
        {
            var oldRecipe = recipes.Where((Recipe arg) => arg.Id == item.Id).FirstOrDefault();
            recipes.Remove(oldRecipe);
            recipes.Add(item);
            return await Task.FromResult(true);
        }
    }
}
