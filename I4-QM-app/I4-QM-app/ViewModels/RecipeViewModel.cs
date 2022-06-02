using I4_QM_app.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class RecipeViewModel : BaseViewModel
    {
        private Recipe _selectedRecipe;

        public ObservableCollection<Recipe> Recipes { get; }
        public Command LoadRecipesCommand { get; }
        public Command<Recipe> RecipeTapped { get; }

        public string RecipesCount { get => Recipes.Count.ToString(); }

        public RecipeViewModel()
        {
            // maybe bad binding atm
            Title = "Recipes";
            Recipes = new ObservableCollection<Recipe>();
            // TODO maybe overloading main thread
            LoadRecipesCommand = new Command(async () => await ExecuteLoadRecipesCommand());

            RecipeTapped = new Command<Recipe>(OnRecipeSelected);
        }

        async Task ExecuteLoadRecipesCommand()
        {
            IsBusy = true;

            try
            {
                Recipes.Clear();
                //var orders = await App.RecipesDataStore.GetItemsAsync(true);

                //foreach (var recipe in recipes)
                //{
                //    //test filter
                //    //if (order.Status == Status.open)
                //    Recipes.Add(recipe);
                //    // TODO sort
                //}


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }



        public void OnAppearing()
        {
            IsBusy = true;
            SelectedRecipe = null;
        }

        public Recipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                SetProperty(ref _selectedRecipe, value);
                OnRecipeSelected(value);
            }
        }

        async void OnRecipeSelected(Recipe item)
        {
            if (item == null)
                return;

            // TODO abstract dialog_service
            //bool answer = await Shell.Current.DisplayAlert("Confirmation", "Start mixing now?", "Yes", "No");

            // This will push the ItemDetailPage onto the navigation stack
            //if (answer) await Shell.Current.GoToAsync($"{nameof(OrderDetailPage)}?{nameof(OrderDetailViewModel.OrderId)}={item.Id}");

        }

        public void AddItemCommand()
        {
        }
    }
}
