using I4_QM_app.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels.Recipes
{
    [QueryProperty(nameof(RecipeId), nameof(RecipeId))]
    public class TransformRecipeViewModel : BaseViewModel
    {
        private string recipeId;
        private Recipe recipe;
        private string id;
        private string creatorId;
        private List<Additive> additives;
        private string name;
        private string description;
        private int used;
        private int amount;
        private int weight;
        private DateTime date;
        private TimeSpan time;

        public Command OrderCommand { get; }
        public Command CancelCommand { get; }
        public Command ClearCommand { get; }

        public TransformRecipeViewModel()
        {
            Title = "Transform";
            OrderCommand = new Command(async () => await TransformRecipe());
            CancelCommand = new Command(OnCancel);
            ClearCommand = new Command(OnClear);
            Date = DateTime.Now;
            Time = DateTime.Now.TimeOfDay;
            Weight = 0;
            Amount = 0;
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

        public List<Additive> Additives
        {
            get => additives;
            set => SetProperty(ref additives, value);
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

        public int Amount
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }

        public int Weight
        {
            get => weight;
            set => SetProperty(ref weight, value);
        }

        public DateTime Date
        {
            get => date;
            set => SetProperty(ref date, value);
        }

        public TimeSpan Time
        {
            get => time;
            set => SetProperty(ref time, value);
        }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private void OnClear()
        {
            Date = DateTime.Now;
            Time = DateTime.Now.TimeOfDay;
            Weight = 0;
            Amount = 0;
        }


        private async void LoadRecipeId(string recipeId)
        {
            try
            {
                var recipe = await App.RecipesDataService.GetItemAsync(recipeId);

                Recipe = recipe;
                Additives = recipe.Additives;
                Name = recipe.Name;
                Description = recipe.Description;

            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        private async Task TransformRecipe()
        {
            bool answer = await App.NotificationService.ShowSimpleDisplayAlert("Confirmation", "Transform into Order?", "Yes", "No");

            if (answer)
            {
                Order newOrder = new Order()
                {
                    Id = Guid.NewGuid().ToString(),
                    Amount = Amount,
                    Weight = Weight,
                    Additives = Additives,
                    Status = Status.open,
                    Received = DateTime.Now,
                    Due = Date.Date.Add(Time)
                };

                //insert order
                await App.OrdersDataService.AddItemAsync(newOrder);

                // update use
                Recipe.Used = Recipe.Used + 1;
                await App.RecipesDataService.UpdateItemAsync(Recipe);

                // navigate
                await Shell.Current.Navigation.PopToRootAsync();
                //await Shell.Current.GoToAsync($"//{nameof(OrdersPage)}");

            }
        }

    }
}
