using I4_QM_app.Models;
using I4_QM_app.Services;
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
    /// <summary>
    /// ViewModel for the Recipe DetailsPage.
    /// </summary>
    [QueryProperty(nameof(RecipeId), nameof(RecipeId))]
    public class RecipeDetailViewModel : BaseViewModel
    {
        private readonly IDataService<Recipe> recipesService;
        private readonly INotificationService notificationService;
        private readonly IDataService<Additive> additivesService;

        private string recipeId;
        private string id;
        private string creatorId;
        private List<Additive> additives;
        private string name;
        private string description;
        private int used;
        private bool available;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeDetailViewModel"/> class.
        /// </summary>
        /// <param name="recipesService">Recipes Service.</param>
        /// <param name="notificationService">Notifications Service.</param>
        /// <param name="additivesService">Additives Service.</param>
        public RecipeDetailViewModel(IDataService<Recipe> recipesService, INotificationService notificationService, IDataService<Additive> additivesService)
        {
            this.recipesService = recipesService;
            this.notificationService = notificationService;
            this.additivesService = additivesService;

            Available = true;
            OrderCommand = new Command(async () => await TransformRecipeAsync(), Validate);
            DeleteCommand = new Command(async () => await DeleteRecipeAsync());
            RefreshCommand = new Command(async () => await LoadRecipeId(RecipeId));
        }

        /// <summary>
        /// Gets command to transform recipe into order.
        /// </summary>
        public Command OrderCommand { get; }

        /// <summary>
        /// Gets command to delete recipe.
        /// </summary>
        public Command DeleteCommand { get; }

        /// <summary>
        /// Gets command to edit recipe.
        /// </summary>
        public Command EditCommand { get; }

        /// <summary>
        /// Gets command to refresh data.
        /// </summary>
        public Command RefreshCommand { get; }

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
        /// Gets or sets the id.
        /// </summary>
        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
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
        /// Gets or sets the creator id.
        /// </summary>
        public string CreatorId
        {
            get => creatorId;
            set => SetProperty(ref creatorId, value);
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
        /// Gets or sets the recipe times used.
        /// </summary>
        public int Used
        {
            get => used;
            set => SetProperty(ref used, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the recipe is available.
        /// </summary>
        public bool Available
        {
            get => available;
            set => SetProperty(ref available, value);
        }

        /// <summary>
        /// Loads recipe from db with id.
        /// </summary>
        /// <param name="recipeId">recipe id.</param>
        /// <returns>Task.</returns>
        private async Task LoadRecipeId(string recipeId)
        {
            IsBusy = true;

            try
            {
                var recipeTemp = await recipesService.GetItemAsync(recipeId);
                var additivesTemp = await additivesService.GetItemsAsync();

                Id = recipeTemp.Id;
                CreatorId = recipeTemp.CreatorId;
                Additives = recipeTemp.Additives;
                Name = recipeTemp.Name;
                Description = recipeTemp.Description;
                Used = recipeTemp.Used;

                foreach (var additive in Additives)
                {
                    Additive item = additivesTemp.FirstOrDefault(x => x.Id == additive.Id);

                    if (item == null)
                    {
                        additive.Available = false;

                        Available = false;
                        OrderCommand.ChangeCanExecute();
                        continue;
                    }

                    additive.Name = item.Name;
                    additive.Available = true;
                    Available = true;
                }

                OrderCommand.ChangeCanExecute();
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Navigate to TransformPage after confirmation.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task TransformRecipeAsync()
        {
            bool answer = await notificationService.ShowSimpleDisplayAlert("Confirmation", "Transform recipe?", "Yes", "No");

            if (answer)
            {
                await Shell.Current.GoToAsync($"{nameof(TransformRecipePage)}?{nameof(TransformRecipeViewModel.RecipeId)}={Id}");
            }
        }

        /// <summary>
        /// Delete recipe after confirmation.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task DeleteRecipeAsync()
        {
            bool answer = await notificationService.ShowSimpleDisplayAlert("Confirmation", "Delete recipe?", "Yes", "No");

            if (answer)
            {
                await recipesService.DeleteItemAsync(Id);
                await Shell.Current.GoToAsync($"//{nameof(RecipesPage)}");
            }
        }

        /// <summary>
        /// Validation if recipe with additives is available.
        /// </summary>
        /// <returns>bool.</returns>
        private bool Validate()
        {
            return Available;
        }
    }
}
