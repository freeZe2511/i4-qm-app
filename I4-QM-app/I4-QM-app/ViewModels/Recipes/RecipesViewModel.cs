using I4_QM_app.Helpers;
using I4_QM_app.Models;
using I4_QM_app.Services;
using I4_QM_app.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    /// <summary>
    /// ViewModel for the Recipes ListPage.
    /// </summary>
    public class RecipesViewModel : BaseViewModel
    {
        private readonly IDataService<Recipe> recipesService;
        private readonly INotificationService notificationService;

        private Recipe selectedRecipe;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipesViewModel"/> class.
        /// </summary>
        /// <param name="recipesService">Recipes Service.</param>
        /// <param name="notificationService">Notifications Service.</param>
        public RecipesViewModel(IDataService<Recipe> recipesService, INotificationService notificationService)
        {
            this.recipesService = recipesService;
            this.notificationService = notificationService;

            Title = "Recipes";
            Descending = true;
            Recipes = new SortableObservableCollection<Recipe>() { SortingSelector = i => i.Id, Descending = Descending };
            LoadRecipesCommand = new Command(async () => await ExecuteLoadRecipesCommand());
            AddItemCommand = new Command(async () => await AddNewItemAsync());
            DeleteAllItemsCommand = new Command(async () => await DeleteAllItemAsync());

            SortByCommand = new Command<string>(
                execute: async (string arg) =>
                {
                    arg = arg.Trim();

                    // works for now
                    if (arg == "Id")
                    {
                        await SortBy(i => i.Id);
                    }

                    if (arg == "Name")
                    {
                        await SortBy(i => i.Name);
                    }

                    if (arg == "CreatorId")
                    {
                        await SortBy(i => i.CreatorId);
                    }

                    if (arg == "Used")
                    {
                        await SortBy(i => i.Used);
                    }

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

        /// <summary>
        /// Gets the recipes collection (sortable).
        /// </summary>
        public SortableObservableCollection<Recipe> Recipes { get; }

        /// <summary>
        /// Gets command to load recipes from db.
        /// </summary>
        public Command LoadRecipesCommand { get; }

        /// <summary>
        /// Gets command to select recipe.
        /// </summary>
        public Command<Recipe> RecipeTapped { get; }

        /// <summary>
        /// Gets command to sort recipes collection.
        /// </summary>
        public Command SortByCommand { get; }

        /// <summary>
        /// Gets command to add recipe.
        /// </summary>
        public Command AddItemCommand { get; }

        /// <summary>
        /// Gets command to disable.
        /// </summary>
        public Command DisableCommand { get; }

        /// <summary>
        /// Gets command to delete all recipes.
        /// </summary>
        public Command DeleteAllItemsCommand { get; }

        /// <summary>
        /// Gets or sets a value indicating whether recipe collection is sorted descending.
        /// </summary>
        public bool Descending { get; set; }

        /// <summary>
        /// Gets or sets the selected reipe.
        /// </summary
        public Recipe SelectedRecipe
        {
            get => selectedRecipe;
            set
            {
                SetProperty(ref selectedRecipe, value);
                OnRecipeSelected(value);
            }
        }

        /// <summary>
        /// Sets IsBusy and selectedRecipe when onAppearing.
        /// </summary>
        public void OnAppearing()
        {
            IsBusy = true;
            SelectedRecipe = null;
        }

        /// <summary>
        /// Sort collection from predicate.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Task.</returns>
        private async Task SortBy(Func<Recipe, object> predicate)
        {
            Recipes.SortingSelector = predicate;
            Recipes.Descending = Descending;
            Descending = !Descending;
            await ExecuteLoadRecipesCommand();
        }

        /// <summary>
        /// Load recipes from db.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task ExecuteLoadRecipesCommand()
        {
            IsBusy = true;

            try
            {
                Recipes.Clear();
                var recipes = await recipesService.GetItemsAsync();

                foreach (var recipe in recipes)
                {
                    Recipes.Add(recipe);
                }
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

        /// <summary>
        /// Navigate to selected recipe detail page.
        /// </summary>
        /// <param name="item">Recipe.</param>
        private async void OnRecipeSelected(Recipe item)
        {
            if (item == null)
            {
                return;
            }

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(RecipeDetailPage)}?{nameof(RecipeDetailViewModel.RecipeId)}={item.Id}");
        }

        /// <summary>
        /// Navigate to new Recipe page.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task AddNewItemAsync()
        {
            bool answer = await notificationService.ShowSimpleDisplayAlert("Confirmation", "Add new Recipe?", "Yes", "No");

            // This will push the ItemDetailPage onto the navigation stack
            if (answer)
            {
                await Shell.Current.GoToAsync($"{nameof(NewRecipePage)}");
            }
        }

        /// <summary>
        /// Delete all recipes.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task DeleteAllItemAsync()
        {
            bool answer = await notificationService.ShowSimpleDisplayAlert("Confirmation", "Delete all recipes?", "Yes", "No");

            if (answer)
            {
                await recipesService.DeleteAllItemsAsync();
            }

            await ExecuteLoadRecipesCommand();
        }
    }
}
