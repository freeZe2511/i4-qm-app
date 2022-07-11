using I4_QM_app.Models;
using I4_QM_app.NUnitTests.MockServices;
using I4_QM_app.Services.Data;
using I4_QM_app.Services.Notifications;
using I4_QM_app.ViewModels.Recipes;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.Tests.Recipes
{
    internal class Recipes
    {
        private IDataService<Recipe> recipesService;
        private INotificationService notificationService;

        [SetUp]
        public void Setup()
        {
            recipesService = new MockRecipesService();
            notificationService = new MockNotificationsService();
        }

        [Test]
        public void RecipesInit()
        {
            var recipes = new RecipesViewModel(recipesService, notificationService);
            Assert.AreEqual(recipes.Title, "Recipes");
            Assert.IsTrue(recipes.Descending);
            Assert.IsNotNull(recipes.Recipes);

            recipes.OnAppearing();
            Assert.IsTrue(recipes.IsBusy);

            Assert.IsNotNull(recipes.DisableCommand);
        }

        [Test]
        public async Task RecipesEmpty()
        {
            var recipes = new RecipesViewModel(recipesService, notificationService);
            Assert.IsEmpty(await recipesService.GetItemsAsync());
            Assert.IsEmpty(recipes.Recipes);
        }

        [Test]
        public async Task RecipesAddData()
        {
            var recipes = new RecipesViewModel(recipesService, notificationService);
            Assert.IsEmpty(await recipesService.GetItemsAsync());
            Assert.IsEmpty(recipes.Recipes);

            var a = new Recipe
            {
                Id = "1",
                Name = "test recipe",
                Description = "test",
                CreatorId = "1234",
                Additives = new List<Additive>()
            };

            await recipesService.AddItemAsync(a);

            recipes.LoadRecipesCommand.Execute(null);
            Assert.AreEqual(1, recipes.Recipes.Count);
        }

        [Test]
        public async Task RecipesSortDataById()
        {
            var recipes = new RecipesViewModel(recipesService, notificationService);

            var a = new Recipe
            {
                Id = "1",
                Name = "test recipe1",
                Description = "test",
                CreatorId = "1234",
                Additives = new List<Additive>(),
                Used = 1
            };

            var b = new Recipe
            {
                Id = "2",
                Name = "test recipe2",
                Description = "test",
                CreatorId = "1234",
                Additives = new List<Additive>(),
                Used = 2
            };

            await recipesService.AddItemAsync(a);
            await recipesService.AddItemAsync(b);

            recipes.LoadRecipesCommand.Execute(null);
            Assert.AreEqual(2, recipes.Recipes.Count);

            Assert.AreEqual(a.Id, recipes.Recipes[0].Id);

            recipes.Descending = false;
            recipes.SortByCommand.Execute("Id");

            Assert.AreEqual(b.Id, recipes.Recipes[0].Id);
        }

        [Test]
        public async Task RecipesSortDataByName()
        {
            var recipes = new RecipesViewModel(recipesService, notificationService);

            var a = new Recipe
            {
                Id = "1",
                Name = "test recipe1",
                Description = "test",
                CreatorId = "1234",
                Additives = new List<Additive>(),
                Used = 1
            };

            var b = new Recipe
            {
                Id = "2",
                Name = "test recipe2",
                Description = "test",
                CreatorId = "1234",
                Additives = new List<Additive>(),
                Used = 2
            };

            await recipesService.AddItemAsync(a);
            await recipesService.AddItemAsync(b);

            recipes.LoadRecipesCommand.Execute(null);
            Assert.AreEqual(2, recipes.Recipes.Count);

            Assert.AreEqual(a.Id, recipes.Recipes[0].Id);

            recipes.Descending = false;
            recipes.SortByCommand.Execute("Name");

            Assert.AreEqual(b.Id, recipes.Recipes[0].Id);
        }

        [Test]
        public async Task RecipesSortDataByCreatorID()
        {
            var recipes = new RecipesViewModel(recipesService, notificationService);

            var a = new Recipe
            {
                Id = "1",
                Name = "test recipe1",
                Description = "test",
                CreatorId = "1234",
                Additives = new List<Additive>(),
                Used = 1
            };

            var b = new Recipe
            {
                Id = "2",
                Name = "test recipe2",
                Description = "test",
                CreatorId = "6789",
                Additives = new List<Additive>(),
                Used = 2
            };

            await recipesService.AddItemAsync(a);
            await recipesService.AddItemAsync(b);

            recipes.LoadRecipesCommand.Execute(null);
            Assert.AreEqual(2, recipes.Recipes.Count);

            Assert.AreEqual(a.Id, recipes.Recipes[0].Id);

            recipes.Descending = false;
            recipes.SortByCommand.Execute("CreatorId");

            Assert.AreEqual(b.Id, recipes.Recipes[0].Id);
        }

        [Test]
        public async Task RecipesSortDataByUsed()
        {
            var recipes = new RecipesViewModel(recipesService, notificationService);

            var a = new Recipe
            {
                Id = "1",
                Name = "test recipe1",
                Description = "test",
                CreatorId = "1234",
                Additives = new List<Additive>(),
                Used = 1
            };

            var b = new Recipe
            {
                Id = "2",
                Name = "test recipe2",
                Description = "test",
                CreatorId = "6789",
                Additives = new List<Additive>(),
                Used = 2
            };

            await recipesService.AddItemAsync(a);
            await recipesService.AddItemAsync(b);

            recipes.LoadRecipesCommand.Execute(null);
            Assert.AreEqual(2, recipes.Recipes.Count);

            Assert.AreEqual(a.Id, recipes.Recipes[0].Id);

            recipes.Descending = false;
            recipes.SortByCommand.Execute("Used");

            Assert.AreEqual(b.Id, recipes.Recipes[0].Id);
        }

        [Test]
        public async Task RecipesRecipeTapped()
        {
            var recipes = new RecipesViewModel(recipesService, notificationService);

            var a = new Recipe
            {
                Id = "1",
                Name = "test recipe1",
                Description = "test",
                CreatorId = "1234",
                Additives = new List<Additive>(),
                Used = 1
            };

            await recipesService.AddItemAsync(a);
            recipes.LoadRecipesCommand.Execute(null);
            Assert.AreEqual(1, recipes.Recipes.Count);

            recipes.RecipeTapped.Execute(a);
            recipes.SelectedRecipe = a;
            Assert.AreEqual(a.Id, recipes.SelectedRecipe.Id);
        }

        [Test]
        public void RecipesAdd()
        {
            var recipes = new RecipesViewModel(recipesService, notificationService);

            var a = new Recipe
            {
                Id = "1",
                Name = "test recipe1",
                Description = "test",
                CreatorId = "1234",
                Additives = new List<Additive>(),
                Used = 1
            };

            recipes.AddItemCommand.Execute(null);
        }

        [Test]
        public async Task RecipesDelete()
        {
            var recipes = new RecipesViewModel(recipesService, notificationService);

            var a = new Recipe
            {
                Id = "1",
                Name = "test recipe1",
                Description = "test",
                CreatorId = "1234",
                Additives = new List<Additive>(),
                Used = 1
            };

            await recipesService.AddItemAsync(a);
            recipes.LoadRecipesCommand.Execute(null);
            Assert.AreEqual(1, recipes.Recipes.Count);

            recipes.DeleteAllItemsCommand.Execute(null);
            Assert.AreEqual(0, recipes.Recipes.Count);
        }
    }
}
