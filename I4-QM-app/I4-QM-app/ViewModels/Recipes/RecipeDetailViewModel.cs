using I4_QM_app.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    [QueryProperty(nameof(RecipeId), nameof(RecipeId))]
    public class RecipeDetailViewModel : BaseViewModel
    {
        private string recipeId;
        private Recipe recipe;
        private string id;
        private string creatorId;
        private List<Additive> additives;
        private string name;
        private string description;
        private int used;

        public Command OrderCommand { get; }
        public Command DeleteCommand { get; }

        public RecipeDetailViewModel()
        {
            //OrderCommand = new Command();
            DeleteCommand = new Command(DeleteRecipe);
        }

        private void DeleteRecipe(object obj)
        {
            throw new NotImplementedException();
        }

        public string RecipeId
        {
            get => recipeId;
            set
            {
                recipeId = value;
                LoadRecipeId(value);
            }
        }

        public Recipe Recipe
        {
            get => recipe;
            set => SetProperty(ref recipe, value);
        }

        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public List<Additive> Additives
        {
            get => additives;
            set => SetProperty(ref additives, value);
        }

        public string CreatorId
        {
            get => creatorId;
            set => SetProperty(ref creatorId, value);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public int Used
        {
            get => used;
            set => SetProperty(ref used, value);
        }

        public async void LoadRecipeId(string recipeId)
        {
            try
            {
                var recipe = await App.RecipesDataStore.GetItemAsync(recipeId);

                Id = recipe.Id;
                CreatorId = null;
                Additives = recipe.Additives;
                Name = recipe.Name;
                Description = recipe.Description;
                Used = recipe.Used;

            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
