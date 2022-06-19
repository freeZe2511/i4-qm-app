using I4_QM_app.Models;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class NewRecipeViewModel : BaseViewModel
    {
        private string name;
        private string description;
        private List<Additive> additivesDatabase;
        private List<Additive> additivesSelected;
        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command ClearCommand { get; }

        public NewRecipeViewModel()
        {

            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            ClearCommand = new Command(OnClear);

            additivesDatabase = new List<Additive>();
            additivesDatabase.Add(new Additive() { Id = "1", Name = "A1" });
            additivesDatabase.Add(new Additive() { Id = "2", Name = "A2" });
            additivesDatabase.Add(new Additive() { Id = "3", Name = "A3" });
            additivesDatabase.Add(new Additive() { Id = "4", Name = "A4" });
            additivesDatabase.Add(new Additive() { Id = "5", Name = "A5" });

            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(Name)
                && !String.IsNullOrWhiteSpace(Description);
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

        public List<Additive> AdditivesDatabase
        {
            get => additivesDatabase;
            set => SetProperty(ref additivesDatabase, value);
        }

        public List<Additive> AdditivesSelected
        {
            get => additivesSelected;
            set => SetProperty(ref additivesSelected, value);
        }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            Recipe newRecipe = new Recipe()
            {
                Id = Guid.NewGuid().ToString(),
                Name = Name,
                Description = Description,
                //CreatorId = UserId,
                Additives = AdditivesDatabase
            };

            await App.RecipesDataStore.AddItemAsync(newRecipe);

            await Shell.Current.GoToAsync("..");

        }

        private async void OnClear()
        {

        }
    }
}
