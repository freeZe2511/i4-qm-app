using I4_QM_app.Models;
using I4_QM_app.ViewModels.Recipes;
using I4_QM_app.Views;
using I4_QM_app.Views.Recipes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
        private bool available;

        public Command OrderCommand { get; }

        public Command DeleteCommand { get; }

        public Command EditCommand { get; }

        public RecipeDetailViewModel()
        {
            // TODO bug???
            Available = true;
            OrderCommand = new Command(async () => await TransformRecipeAsync(), Validate);
            DeleteCommand = new Command(async () => await DeleteRecipeAsync());
        }

        private async Task TransformRecipeAsync()
        {
            bool answer = await App.NotificationService.ShowSimpleDisplayAlert("Confirmation", "Transform recipe?", "Yes", "No");

            if (answer)
            {
                await Shell.Current.GoToAsync($"{nameof(TransformRecipePage)}?{nameof(TransformRecipeViewModel.RecipeId)}={Id}");
            }
        }

        private async Task DeleteRecipeAsync()
        {
            bool answer = await App.NotificationService.ShowSimpleDisplayAlert("Confirmation", "Delete recipe?", "Yes", "No");

            if (answer)
            {
                await App.RecipesDataService.DeleteItemAsync(Id);
                await Shell.Current.GoToAsync($"//{nameof(RecipesPage)}");
            }
        }

        private bool Validate()
        {
            return Available;
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

        public bool Available
        {
            get => available;
            set => SetProperty(ref available, value);
        }

        public async void LoadRecipeId(string recipeId)
        {
            try
            {
                var recipe = await App.RecipesDataService.GetItemAsync(recipeId);
                var additives = await App.AdditivesDataService.GetItemsAsync();

                Id = recipe.Id;
                CreatorId = recipe.CreatorId;
                Additives = recipe.Additives;
                Name = recipe.Name;
                Description = recipe.Description;
                Used = recipe.Used;

                foreach (var additive in Additives)
                {
                    Additive item = additives.FirstOrDefault(x => x.Id == additive.Id);

                    if (item == null)
                    {
                        additive.Available = false;

                        Available = false;
                        continue;
                    }

                    additive.Available = true;
                }

            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
