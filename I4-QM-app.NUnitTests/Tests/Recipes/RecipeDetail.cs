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
    internal class RecipeDetail
    {
        private IDataService<Recipe> recipesService;
        private INotificationService notificationService;
        private IDataService<Additive> additivesService;

        [SetUp]
        public void Setup()
        {
            recipesService = new MockRecipesService();
            notificationService = new MockNotificationsService();
            additivesService = new MockAdditivesService();
        }

        [Test]
        public void RecipeDetailInit()
        {
            var recipes = new RecipeDetailViewModel(recipesService, notificationService, additivesService);
            Assert.IsTrue(recipes.Available);
            Assert.IsNotNull(recipes.RefreshCommand);
            Assert.IsNotNull(recipes.OrderCommand);
        }

        [Test]
        public void RecipeDetailInitWithRecipeID()
        {
            var b = new Additive
            {
                Id = "2",
                Portion = 100,
                Name = "test2"
            };

            var additives = new List<Additive>();
            additives.Add(b);

            var a = new Recipe
            {
                Id = "1",
                Name = "test recipe1",
                Description = "test",
                CreatorId = "1234",
                Additives = additives,
                Used = 1
            };

            recipesService.AddItemAsync(a);

            var recipes = new RecipeDetailViewModel(recipesService, notificationService, additivesService);
            recipes.RecipeId = "1";

            Assert.AreEqual(a.Id, recipes.RecipeId);
            Assert.AreEqual(a.Id, recipes.Id);
            Assert.AreEqual(a.Additives, recipes.Additives);
            Assert.AreEqual(a.CreatorId, recipes.CreatorId);
            Assert.AreEqual(a.Name, recipes.Name);
            Assert.AreEqual(a.Description, recipes.Description);
            Assert.AreEqual(a.Used, recipes.Used);
        }

        [Test]
        public void RecipeDetailAdditiveUnavailable()
        {
            var b = new Additive
            {
                Id = "2",
                Portion = 100,
                Name = "test2"
            };

            var c = new Additive
            {
                Id = "3",
                Portion = 10,
                Name = "test3"
            };

            var additives = new List<Additive>();
            additives.Add(b);
            additives.Add(c);

            var a = new Recipe
            {
                Id = "1",
                Name = "test recipe1",
                Description = "test",
                CreatorId = "1234",
                Additives = additives,
                Used = 1
            };

            recipesService.AddItemAsync(a);

            var recipes = new RecipeDetailViewModel(recipesService, notificationService, additivesService);
            recipes.RecipeId = "1";

            Assert.IsFalse(recipes.OrderCommand.CanExecute(null));

        }

        [Test]
        public void RecipeDetailAdditiveAvailable()
        {
            var b = new Additive
            {
                Id = "2",
                Portion = 100,
                Name = "test2"
            };

            var c = new Additive
            {
                Id = "3",
                Portion = 10,
                Name = "test3"
            };

            var additives = new List<Additive>();
            additives.Add(b);
            additives.Add(c);

            var a = new Recipe
            {
                Id = "1",
                Name = "test recipe1",
                Description = "test",
                CreatorId = "1234",
                Additives = additives,
                Used = 1
            };

            recipesService.AddItemAsync(a);
            additivesService.AddItemAsync(b);
            additivesService.AddItemAsync(c);

            var recipes = new RecipeDetailViewModel(recipesService, notificationService, additivesService);
            recipes.RecipeId = "1";

            Assert.IsTrue(recipes.OrderCommand.CanExecute(null));
        }

        [Test]
        public void RecipeDetailTransform()
        {
            var b = new Additive
            {
                Id = "2",
                Portion = 100,
                Name = "test2"
            };

            var additives = new List<Additive>();
            additives.Add(b);

            var a = new Recipe
            {
                Id = "1",
                Name = "test recipe1",
                Description = "test",
                CreatorId = "1234",
                Additives = additives,
                Used = 1
            };

            recipesService.AddItemAsync(a);
            additivesService.AddItemAsync(b);

            var recipes = new RecipeDetailViewModel(recipesService, notificationService, additivesService);
            recipes.RecipeId = "1";

            Assert.IsTrue(recipes.OrderCommand.CanExecute(null));
            recipes.OrderCommand.Execute(null);
        }

        [Test]
        public async Task RecipeDetailDeleteAsync()
        {
            var additives = new List<Additive>();

            var a = new Recipe
            {
                Id = "1",
                Name = "test recipe1",
                Description = "test",
                CreatorId = "1234",
                Additives = additives,
                Used = 1
            };

            await recipesService.AddItemAsync(a);

            var recipes = new RecipeDetailViewModel(recipesService, notificationService, additivesService);
            recipes.RecipeId = "1";

            Assert.AreEqual(a, await recipesService.GetItemAsync("1"));
            recipes.DeleteCommand.Execute(null);
            Assert.AreEqual(null, await recipesService.GetItemAsync("1"));
        }

    }
}
