using I4_QM_app.Models;
using I4_QM_app.NUnitTests.MockServices;
using I4_QM_app.Services;
using I4_QM_app.ViewModels.Recipes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.Tests.Recipes
{
    internal class Transform
    {
        private IDataService<Recipe> recipesService;
        private INotificationService notificationService;
        private IDataService<Order> ordersService;

        [SetUp]
        public void Setup()
        {
            recipesService = new MockRecipesService();
            notificationService = new MockNotificationsService();
            ordersService = new MockOrderService();
        }

        [Test]
        public void TransformInit()
        {
            var transform = new TransformRecipeViewModel(ordersService, notificationService, recipesService);
            Assert.AreEqual(transform.Title, "Transform");
            Assert.IsNotNull(transform.OrderCommand);
            Assert.AreEqual(0, transform.Weight);
            Assert.AreEqual(0, transform.Amount);
        }

        [Test]
        public void TransformInitRecipeID()
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

            var transform = new TransformRecipeViewModel(ordersService, notificationService, recipesService);
            transform.RecipeId = "1";

            Assert.AreEqual(a.Id, transform.RecipeId);
            Assert.AreEqual(a.Name, transform.Name);
            Assert.AreEqual(a.Additives, transform.Additives);
            Assert.AreEqual(a.Description, transform.Description);
        }

        [Test]
        public void TransformClearCancel()
        {
            var transform = new TransformRecipeViewModel(ordersService, notificationService, recipesService);

            transform.Weight = 3;
            transform.Amount = 100;

            transform.ClearCommand.Execute(null);
            Assert.AreEqual(0, transform.Weight);
            Assert.AreEqual(0, transform.Amount);

            transform.CancelCommand.Execute(null);
        }

        [Test]
        public void TransformUpdate()
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

            var transform = new TransformRecipeViewModel(ordersService, notificationService, recipesService);
            transform.RecipeId = "1";

            Assert.IsFalse(transform.OrderCommand.CanExecute(null));

            transform.Date = DateTime.Now.AddDays(5);
            transform.Time = DateTime.Now.TimeOfDay;
            transform.Weight = 3;
            transform.Amount = 100;

            transform.UpdateCommand.Execute(null);
            Assert.IsTrue(transform.OrderCommand.CanExecute(null));
        }

        [Test]
        public async Task TransformTransformSuccess()
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

            await recipesService.AddItemAsync(a);

            var transform = new TransformRecipeViewModel(ordersService, notificationService, recipesService);
            transform.RecipeId = "1";

            Assert.IsFalse(transform.OrderCommand.CanExecute(null));

            transform.Date = DateTime.Now.AddDays(4);
            transform.Time = DateTime.Now.TimeOfDay;
            transform.Weight = 3;
            transform.Amount = 100;

            transform.OrderCommand.Execute(null);

            Assert.AreEqual(1, (await ordersService.GetItemsAsync()).Count());
            Assert.AreEqual(2, (await recipesService.GetItemAsync(a.Id)).Used);
        }

        [Test]
        public async Task TransformTransformNoSuccess()
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

            await recipesService.AddItemAsync(a);

            var transform = new TransformRecipeViewModel(ordersService, notificationService, recipesService);
            transform.RecipeId = "1";

            Assert.IsFalse(transform.OrderCommand.CanExecute(null));

            transform.Date = DateTime.Now;
            transform.Time = DateTime.Now.TimeOfDay;
            transform.Weight = 3;
            transform.Amount = 100;

            transform.OrderCommand.Execute(null);

            Assert.AreEqual(0, (await ordersService.GetItemsAsync()).Count());
            Assert.AreEqual(1, (await recipesService.GetItemAsync(a.Id)).Used);
        }

    }
}
