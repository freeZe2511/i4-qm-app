using I4_QM_app.Models;
using I4_QM_app.Services.Data;
using I4_QM_app.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels.Recipes
{
    /// <summary>
    /// ViewModel of the RecipeTransformationPage.
    /// </summary>
    [QueryProperty(nameof(RecipeId), nameof(RecipeId))]
    public class TransformRecipeViewModel : BaseViewModel
    {
        private readonly IDataService<Order> ordersService;
        private readonly INotificationService notificationService;
        private readonly IDataService<Recipe> recipesService;

        private string recipeId;
        private Recipe recipe;
        private List<Additive> additives;
        private string name;
        private string description;
        private int amount;
        private int weight;
        private DateTime date;
        private TimeSpan time;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformRecipeViewModel"/> class.
        /// </summary>
        /// <param name="ordersService">Orders Service.</param>
        /// <param name="notificationService">Notifications Service.</param>
        /// <param name="recipesService">Recipes Service.</param>
        public TransformRecipeViewModel(IDataService<Order> ordersService, INotificationService notificationService, IDataService<Recipe> recipesService)
        {
            this.ordersService = ordersService;
            this.notificationService = notificationService;
            this.recipesService = recipesService;

            Title = "Transform";
            OrderCommand = new Command(async () => await TransformRecipe(), Validate);
            CancelCommand = new Command(OnCancel);
            ClearCommand = new Command(OnClear);
            UpdateCommand = new Command(OnUpdate);
            Date = DateTime.Now.AddDays(1);
            Time = DateTime.Now.TimeOfDay;
            Weight = 0;
            Amount = 0;
        }

        /// <summary>
        /// Gets command to finish transformation.
        /// </summary>
        public Command OrderCommand { get; }

        /// <summary>
        /// Gets command to cancel transformation.
        /// </summary>
        public Command CancelCommand { get; }

        /// <summary>
        /// Gets command to clear transformation form.
        /// </summary>
        public Command ClearCommand { get; }

        /// <summary>
        /// Gets command to update to UI.
        /// </summary>
        public Command UpdateCommand { get; }

        /// <summary>
        /// Gets or sets the recipe id.
        /// </summary>
        public string RecipeId
        {
            get => recipeId;
            set
            {
                recipeId = value;
                LoadRecipeId(value);
            }
        }

        /// <summary>
        /// Gets or sets the recipe.
        /// </summary>
        public Recipe Recipe
        {
            get => recipe;
            set => SetProperty(ref recipe, value);
        }

        /// <summary>
        /// Gets or sets the additives list.
        /// </summary>
        public List<Additive> Additives
        {
            get => additives;
            set => SetProperty(ref additives, value);
        }

        /// <summary>
        /// Gets or sets the recipe name.
        /// </summary>
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        /// <summary>
        /// Gets or sets the recipe description.
        /// </summary>
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        /// <summary>
        /// Gets or sets the total amount.
        /// </summary>
        public int Amount
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }

        /// <summary>
        /// Gets or sets the single item weight.
        /// </summary>
        public int Weight
        {
            get => weight;
            set => SetProperty(ref weight, value);
        }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        public DateTime Date
        {
            get => date;
            set => SetProperty(ref date, value);
        }

        /// <summary>
        /// Gets or sets the due time.
        /// </summary>
        public TimeSpan Time
        {
            get => time;
            set => SetProperty(ref time, value);
        }

        /// <summary>
        /// Navigates back.
        /// </summary>
        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        /// <summary>
        /// Clears transformation form.
        /// </summary>
        private void OnClear()
        {
            Date = DateTime.Now.AddDays(1);
            Time = DateTime.Now.TimeOfDay;
            Weight = 0;
            Amount = 0;
        }

        /// <summary>
        /// Handles UI updates.
        /// </summary>
        private void OnUpdate()
        {
            OrderCommand.ChangeCanExecute();
        }

        /// <summary>
        /// Validation of form if transformation is valid.
        /// </summary>
        /// <returns>bool.</returns>
        private bool Validate()
        {
            return Weight > 0 && Amount > 0 && Date.Date.Add(Time) >= DateTime.Now;
        }

        /// <summary>
        /// Loads recipe from db for transformation into order.
        /// </summary>
        /// <param name="recipeId">Recipe Id.</param>
        /// <returns>Task.</returns>
        private async Task LoadRecipeId(string recipeId)
        {
            try
            {
                var recipeTemp = await recipesService.GetItemAsync(recipeId);

                Recipe = recipeTemp;
                Additives = recipeTemp.Additives;
                Name = recipeTemp.Name;
                Description = recipeTemp.Description;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        /// <summary>
        /// Handles transformation of recipe into order.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task TransformRecipe()
        {
            if (!(Amount > 0 && Weight > 0 && Date.Date.Add(Time) > DateTime.Now))
            {
                return;
            }

            bool answer = await notificationService.ShowSimpleDisplayAlert("Confirmation", "Transform into Order?", "Yes", "No");

            if (answer)
            {
                Order newOrder = new Order()
                {
                    Id = Guid.NewGuid().ToString(),
                    Amount = Amount,
                    Weight = Weight,
                    Additives = Additives,
                    Status = Status.Open,
                    Received = DateTime.Now,
                    Due = Date.Date.Add(Time),
                };

                // insert order
                await ordersService.AddItemAsync(newOrder);

                // update use
                Recipe.Used = Recipe.Used + 1;
                await recipesService.UpdateItemAsync(Recipe);

                // navigate
                await Shell.Current.Navigation.PopToRootAsync();
            }
        }
    }
}
