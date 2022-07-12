using I4_QM_app.Models;
using I4_QM_app.NUnitTests.MockServices;
using I4_QM_app.Services.Abstract;
using I4_QM_app.Services.Connection;
using I4_QM_app.Services.Data;
using I4_QM_app.Services.Notifications;
using I4_QM_app.ViewModels.Recipes;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.Tests.Recipes
{
    internal class NewRecipe
    {
        private IDataService<Recipe> recipesService;
        private INotificationService notificationsService;
        private IConnectionService connectionService;
        private IDataService<Additive> additivesService;
        private IAbstractService abstractService;

        [SetUp]
        public void Setup()
        {
            recipesService = new MockRecipesService();
            notificationsService = new MockNotificationsService();
            connectionService = new MockConnectionService();
            additivesService = new MockAdditivesService();
            abstractService = new MockAbstractService();
        }

        [Test]
        public void NewRecipeInit()
        {
            var newRecipe = new NewRecipeViewModel(notificationsService, connectionService, additivesService, recipesService, abstractService);
            Assert.IsNotNull(newRecipe.CancelCommand);
            Assert.IsNotNull(newRecipe.UpdateCommand);

            Assert.IsFalse(newRecipe.SaveCommand.CanExecute(null));

            newRecipe.CancelCommand.Execute(null);
            newRecipe.OnAppearing();
            Assert.IsTrue(newRecipe.IsBusy);
        }

        [Test]
        public void NewRecipeClear()
        {
            var newRecipe = new NewRecipeViewModel(notificationsService, connectionService, additivesService, recipesService, abstractService);
            Assert.IsNotNull(newRecipe.CancelCommand);
            Assert.IsNotNull(newRecipe.UpdateCommand);

            newRecipe.Name = "Test Recipe";
            newRecipe.Description = "test";
            Assert.IsNotEmpty(newRecipe.Name);

            newRecipe.ClearCommand.Execute(null);
            Assert.IsEmpty(newRecipe.Name);
            Assert.IsEmpty(newRecipe.Description);
        }

        [Test]
        public void NewRecipeRefresh()
        {
            var newRecipe = new NewRecipeViewModel(notificationsService, connectionService, additivesService, recipesService, abstractService);

            Assert.IsFalse(newRecipe.Additives.Any());

            var a = new Additive
            {
                Id = "1",
                Portion = 5,
                Name = "test1"
            };

            additivesService.AddItemAsync(a);

            newRecipe.RefreshCommand.Execute(null);

            Assert.IsTrue(newRecipe.Additives.Any());
        }

        [Test]
        public void NewRecipeRefreshAlreadyData()
        {
            var newRecipe = new NewRecipeViewModel(notificationsService, connectionService, additivesService, recipesService, abstractService);

            var a = new Additive
            {
                Id = "1",
                Portion = 5,
                Name = "test1"
            };

            additivesService.AddItemAsync(a);
            newRecipe.RefreshCommand.Execute(null);

            newRecipe.Additives[0].Portion = 5;
            newRecipe.Additives[0].Checked = true;

            var b = new Additive
            {
                Id = "2",
                Portion = 100,
                Name = "test2"
            };

            additivesService.AddItemAsync(b);
            newRecipe.RefreshCommand.Execute(null);

            Assert.AreEqual(5, newRecipe.Additives[0].Portion);
            Assert.IsTrue(newRecipe.Additives[0].Checked);

        }

        [Test]
        public async Task NewRecipeSaveSuccessAsync()
        {
            var newRecipe = new NewRecipeViewModel(notificationsService, connectionService, additivesService, recipesService, abstractService);

            var a = new Additive
            {
                Id = "1",
                Portion = 5,
                Name = "test1"
            };

            await additivesService.AddItemAsync(a);
            newRecipe.RefreshCommand.Execute(null);

            newRecipe.Name = "test recipe";
            newRecipe.Description = "test";
            newRecipe.Additives[0].Portion = 5;
            newRecipe.Additives[0].Checked = true;

            newRecipe.UpdateCommand.Execute(null);
            Assert.IsTrue(newRecipe.SaveCommand.CanExecute(null));

            newRecipe.SaveCommand.Execute(null);
            Assert.IsTrue((await recipesService.GetItemsAsync()).Any());
        }

        [Test]
        public void NewRecipeSaveNotChecked()
        {
            var newRecipe = new NewRecipeViewModel(notificationsService, connectionService, additivesService, recipesService, abstractService);

            var a = new Additive
            {
                Id = "1",
                Portion = 5,
                Name = "test1"
            };

            additivesService.AddItemAsync(a);
            newRecipe.RefreshCommand.Execute(null);

            newRecipe.Name = "test recipe";
            newRecipe.Description = "test";
            newRecipe.Additives[0].Portion = 5;
            newRecipe.Additives[0].Checked = false;

            newRecipe.UpdateCommand.Execute(null);
            Assert.IsFalse(newRecipe.SaveCommand.CanExecute(null));
        }

        [Test]
        public void NewRecipeSaveNoPortion()
        {
            var newRecipe = new NewRecipeViewModel(notificationsService, connectionService, additivesService, recipesService, abstractService);

            var a = new Additive
            {
                Id = "1",
                Portion = 5,
                Name = "test1"
            };

            additivesService.AddItemAsync(a);
            newRecipe.RefreshCommand.Execute(null);

            newRecipe.Name = "test recipe";
            newRecipe.Description = "test";
            newRecipe.Additives[0].Portion = 0;
            newRecipe.Additives[0].Checked = true;

            newRecipe.UpdateCommand.Execute(null);
            Assert.IsFalse(newRecipe.SaveCommand.CanExecute(null));
        }

        [Test]
        public void NewRecipeSaveNoName()
        {
            var newRecipe = new NewRecipeViewModel(notificationsService, connectionService, additivesService, recipesService, abstractService);

            var a = new Additive
            {
                Id = "1",
                Portion = 5,
                Name = "test1"
            };

            additivesService.AddItemAsync(a);
            newRecipe.RefreshCommand.Execute(null);

            newRecipe.UpdateCommand.Execute(null);
            Assert.IsFalse(newRecipe.SaveCommand.CanExecute(null));
        }

        [Test]
        public void NewRecipeSaveNoAdditives()
        {
            var newRecipe = new NewRecipeViewModel(notificationsService, connectionService, additivesService, recipesService, abstractService);

            newRecipe.UpdateCommand.Execute(null);
            Assert.IsFalse(newRecipe.SaveCommand.CanExecute(null));
        }
    }
}
