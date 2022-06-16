using I4_QM_app.Helpers;
using I4_QM_app.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class RecipeViewModel : BaseViewModel
    {
        private Recipe _selectedRecipe;

        public SortableObservableCollection<Recipe> Recipes { get; }
        public Command LoadRecipesCommand { get; }
        public Command<Recipe> RecipeTapped { get; }
        public Command SortByCommand { get; }
        public Command DisableCommand { get; }

        public bool Descending { get; set; }


        public RecipeViewModel()
        {
            // maybe bad binding atm
            Title = "Recipes";
            Descending = true;
            Recipes = new SortableObservableCollection<Recipe>() { SortingSelector = i => i.Id, Descending = Descending };
            // TODO maybe overloading main thread
            LoadRecipesCommand = new Command(async () => await ExecuteLoadRecipesCommand());

            SortByCommand = new Command<string>(
                execute: async (string arg) =>
                {
                    arg = arg.Trim();

                    // works
                    if (arg == "Id") await SortBy(i => i.Id);
                    //if (arg == "Due") await SortBy(i => i.Due);
                    //if (arg == "Amount") await SortBy(i => i.Amount);
                    //if (arg == "Created") await SortBy(i => i.Created);


                    // https://stackoverflow.com/questions/16213005/how-to-convert-a-lambdaexpression-to-typed-expressionfunct-t
                    // only works with id?! Specified cast is not valid
                    //if (typeof(Order).GetProperty(arg) != null)
                    //{
                    //    ParameterExpression parameter = Expression.Parameter(typeof(Order), "i");
                    //    MemberExpression memberExpression = Expression.Property(parameter, typeof(Order).GetProperty(arg));
                    //    LambdaExpression lambda = Expression.Lambda(memberExpression, parameter);

                    //    Console.WriteLine(lambda.ToString());

                    //    await SortBy((Func<Order, object>)lambda.Compile());
                    //}

                    // https://stackoverflow.com/questions/10655761/convert-string-into-func
                    // compiler error
                    //var str = "i => i." + arg;
                    //Console.WriteLine(str);
                    //var func = await CSharpScript.EvaluateAsync<Func<Order, object>>(str);
                    //await SortBy(func);


                });

            DisableCommand = new Command(execute: () => { }, canExecute: () => { return false; });

            RecipeTapped = new Command<Recipe>(OnRecipeSelected);
        }

        private async Task SortBy(Func<Recipe, object> predicate)
        {
            Recipes.SortingSelector = predicate;
            Recipes.Descending = Descending;
            Descending = !Descending;
            await ExecuteLoadRecipesCommand();
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
