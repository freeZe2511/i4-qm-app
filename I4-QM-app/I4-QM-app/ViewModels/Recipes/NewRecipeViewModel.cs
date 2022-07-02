using I4_QM_app.Models;
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
    public class NewRecipeViewModel : BaseViewModel
    {
        private string name;
        private string description;
        private ObservableCollection<Additive> additives;
        private ObservableCollection<Additive> oldAdditives;

        public Command SaveCommand { get; }

        public Command CancelCommand { get; }

        public Command ClearCommand { get; }

        public Command UpdateCommand { get; }

        public Command RefreshCommand { get; }

        public NewRecipeViewModel()
        {
            Additives = new ObservableCollection<Additive>();
            oldAdditives = new ObservableCollection<Additive>();
            SaveCommand = new Command(OnSave, Validate);
            CancelCommand = new Command(OnCancel);
            ClearCommand = new Command(OnClear);
            UpdateCommand = new Command(OnUpdate);
            RefreshCommand = new Command(async () => await LoadRefreshCommand());
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        private async Task LoadRefreshCommand()
        {
            IsBusy = true;

            try
            {
                var fs = App.DB.GetStorage<string>("myImages");
                var list = await App.AdditivesDataService.GetItemsAsync();

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

                    LiteFileInfo<string> file = fs.FindById(item.Id);

                    if (file != null)
                    {
                        item.Image = ImageSource.FromStream(() => file.OpenRead());
                    }
                    else
                    {
                        item.Image = ImageSource.FromFile("no_image.png");
                    }

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

        private void OnUpdate()
        {
            SaveCommand.ChangeCanExecute();
        }

        private bool Validate()
        {
            return !String.IsNullOrWhiteSpace(Name)
                && !String.IsNullOrWhiteSpace(Description)
                && Additives.Count(i => (i.Checked && i.Portion > 0)) >= 1
                && !Additives.Any(i => !i.Checked && i.Portion > 0)
                && !Additives.Any(i => i.Checked && i.Portion <= 0);
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

        public ObservableCollection<Additive> Additives
        {
            get => additives;
            set => SetProperty(ref additives, value);
        }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            bool answer = await App.NotificationService.ShowSimpleDisplayAlert("Confirmation", "Save?", "Yes", "No");

            if (answer)
            {
                Recipe newRecipe = new Recipe()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Name,
                    Description = Description,
                    CreatorId = Preferences.Get("UserID", string.Empty),
                    Additives = Additives.Where(i => i.Checked).ToList(),
                };

                await App.RecipesDataService.AddItemAsync(newRecipe);

                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                };

                string res = System.Text.Json.JsonSerializer.Serialize<Recipe>(newRecipe, options);
                await App.ConnectionService.HandlePublishMessage("recipes/new", res);

                await Shell.Current.GoToAsync("..");
            }

        }


        private async void OnClear()
        {
            Name = string.Empty;
            Description = string.Empty;
            Additives.Clear();
            await LoadRefreshCommand();
        }
    }
}
