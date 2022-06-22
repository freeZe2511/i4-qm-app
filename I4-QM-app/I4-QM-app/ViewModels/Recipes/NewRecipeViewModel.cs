using I4_QM_app.Models;
using System;
using System.Collections.Generic;
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
        private List<Additive> additives;

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command ClearCommand { get; }

        public NewRecipeViewModel()
        {

            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            ClearCommand = new Command(OnClear);

            additives = new List<Additive>();

            _ = Task.Run(async () => await GetAdditives());

            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        private async Task GetAdditives()
        {
            Additives.Clear();

            var list = await App.AdditivesDataStore.GetItemsAsync();
            foreach (Additive item in list)
            {
                Additives.Add(item);
            }
        }


        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(Name)
                && !String.IsNullOrWhiteSpace(Description);
            //&& Additives.FindAll(i => i.Checked == true).Count > 0;
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

        public List<Additive> Additives
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
                    CreatorId = Preferences.Get("UserID", null),
                    Additives = Additives.FindAll(i => i.Checked == true)
                };

                await App.RecipesDataStore.AddItemAsync(newRecipe);

                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                };

                string res = JsonSerializer.Serialize<Recipe>(newRecipe, options);
                await App.ConnectionService.HandlePublishMessage("recipes/new", res);

                await Shell.Current.GoToAsync("..");
            }

        }


        private async void OnClear()
        {
            Name = "";
            Description = "";

            // TODO reset additives?

            //await GetAdditives();

            //foreach (Additive additive in Additives)
            //{
            //    Console.WriteLine(additive.Name);
            //    additive.Checked = false;
            //    additive.Portion = 0;
            //}

        }
    }
}
