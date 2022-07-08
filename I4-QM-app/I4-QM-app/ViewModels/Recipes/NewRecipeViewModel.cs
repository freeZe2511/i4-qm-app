using I4_QM_app.Models;
using I4_QM_app.Services;
using LiteDB;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    /// <summary>
    /// ViewModel for New Recipe Page.
    /// </summary>
    public class NewRecipeViewModel : BaseViewModel
    {
        private readonly IDataService<Recipe> recipesService;
        private readonly INotificationService notificationService;
        private readonly IConnectionService connectionService;
        private readonly IDataService<Additive> additivesService;

        private string name;
        private string description;
        private ObservableCollection<Additive> additives;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewRecipeViewModel"/> class.
        /// </summary>
        /// <param name="notificationService">Notifications Service.</param>
        /// <param name="connectionService">Connection Service.</param>
        /// <param name="additivesService">Additives Service.</param>
        /// <param name="recipesService">Recipes Service.</param>
        public NewRecipeViewModel(INotificationService notificationService, IConnectionService connectionService, IDataService<Additive> additivesService, IDataService<Recipe> recipesService)
        {
            this.recipesService = recipesService;
            this.notificationService = notificationService;
            this.connectionService = connectionService;
            this.additivesService = additivesService;

            Additives = new ObservableCollection<Additive>();
            SaveCommand = new Command(OnSave, Validate);
            CancelCommand = new Command(OnCancel);
            ClearCommand = new Command(OnClear);
            UpdateCommand = new Command(OnUpdate);
            RefreshCommand = new Command(async () => await LoadRefreshCommand());
        }

        /// <summary>
        /// Gets command to save new recipe.
        /// </summary>
        public Command SaveCommand { get; }

        /// <summary>
        /// Gets command to cancel.
        /// </summary>
        public Command CancelCommand { get; }

        /// <summary>
        /// Gets command to clear new recipe form.
        /// </summary>
        public Command ClearCommand { get; }

        /// <summary>
        /// Gets command to handle UI updates.
        /// </summary>
        public Command UpdateCommand { get; }

        /// <summary>
        /// Gets command to refresh data.
        /// </summary>
        public Command RefreshCommand { get; }

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
        /// Gets or sets the recipe additives list.
        /// </summary>
        public ObservableCollection<Additive> Additives
        {
            get => additives;
            set => SetProperty(ref additives, value);
        }

        /// <summary>
        /// Sets IsBusy when onAppearing.
        /// </summary>
        public void OnAppearing()
        {
            IsBusy = true;
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
        /// Saves new recipe after confirmation, sends mqtt to broker and saves in db.
        /// </summary>
        private async void OnSave()
        {
            bool answer = await notificationService.ShowSimpleDisplayAlert("Confirmation", "Save?", "Yes", "No");

            if (answer)
            {
                Additives.ToList().ForEach(i => i.Image = null);

                Recipe newRecipe = new Recipe()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Name,
                    Description = Description,
                    CreatorId = Preferences.Get("UserID", string.Empty),
                    Additives = Additives.Where(i => i.Checked).ToList(),
                };

                await recipesService.AddItemAsync(newRecipe);

                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                };

                string res = System.Text.Json.JsonSerializer.Serialize<Recipe>(newRecipe, options);
                await connectionService.HandlePublishMessage("recipes/new", res);

                await Shell.Current.GoToAsync("..");
            }
        }

        /// <summary>
        /// Load additives and images from db.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task LoadRefreshCommand()
        {
            IsBusy = true;

            try
            {
                var fs = App.DB.GetStorage<string>("myImages");
                var list = await additivesService.GetItemsAsync();

                if (!list.Any())
                {
                    Additives.Clear();
                }

                foreach (var item in list)
                {
                    var oldItem = Additives.FirstOrDefault(x => x.Id == item.Id);

                    if (oldItem != null)
                    {
                        item.Portion = oldItem.Portion;
                        item.Checked = oldItem.Checked;
                        Additives.Remove(oldItem);
                    }

                    item.Image = GetAdditivesImage(fs, item.Id);
                    Additives.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Get additives image from db.
        /// </summary>
        /// <param name="fs">FileStorage.</param>
        /// <param name="id">id.</param>
        /// <returns>ImageSource.</returns>
        private ImageSource GetAdditivesImage(ILiteStorage<string> fs, string id)
        {
            LiteFileInfo<string> file = fs.FindById(id);

            if (file != null)
            {
                return ImageSource.FromStream(() => file.OpenRead());
            }
            else
            {
                return ImageSource.FromFile("no_image.png");
            }
        }

        /// <summary>
        /// Reset new recipe form.
        /// </summary>
        private async void OnClear()
        {
            Name = string.Empty;
            Description = string.Empty;
            Additives.Clear();
            await LoadRefreshCommand();
        }

        /// <summary>
        /// Handle canExecute updates from ui.
        /// </summary>
        private void OnUpdate()
        {
            SaveCommand.ChangeCanExecute();
        }

        /// <summary>
        /// Validation to check new recipe form.
        /// </summary>
        /// <returns>bool.</returns>
        private bool Validate()
        {
            return !string.IsNullOrWhiteSpace(Name)
                && !string.IsNullOrWhiteSpace(Description)
                && Additives.Any(i => (i.Checked && i.Portion > 0))
                && !Additives.Any(i => !i.Checked && i.Portion > 0)
                && !Additives.Any(i => i.Checked && i.Portion <= 0);
        }
    }
}
