using I4_QM_app.Models;
using I4_QM_app.NUnitTests.MockServices;
using I4_QM_app.Services;
using I4_QM_app.Services.Abstract;
using I4_QM_app.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace I4_QM_app.NUnitTests.Tests
{
    internal class Home
    {
        private IDataService<Order> ordersService;
        private IDataService<Recipe> recipesService;
        private IDataService<Additive> additivesService;
        private IAbstractService abstractService;

        [SetUp]
        public void Setup()
        {
            ordersService = new MockOrderService();
            recipesService = new MockRecipesService();
            additivesService = new MockAdditivesService();
            abstractService = new MockAbstractService();
            abstractService.SetPreferences("UserID", "1234");
        }

        [Test]
        public void HomeInit()
        {
            var home = new HomeViewModel(ordersService, recipesService, additivesService, abstractService);
            Assert.IsNotNull(home);
            Assert.AreEqual(home.Title, "Home");

            home.OnAppearing();
            Assert.IsTrue(home.IsBusy);
            home.RefreshCommand.Execute(null);
            Assert.AreEqual(home.UserId, "1234");

            abstractService.ClearPreferences();
        }

        [Test]
        public void HomeLoadDataEmpty()
        {
            var home = new HomeViewModel(ordersService, recipesService, additivesService, abstractService);
            Assert.IsNotNull(home);
            home.RefreshCommand.Execute(null);

            Assert.AreEqual(home.OpenOrdersCount, 0);
            Assert.AreEqual(home.MixedOrdersCount, 0);
            Assert.AreEqual(home.RatedOrdersCount, 0);
            Assert.AreEqual(home.NextOrder, DateTime.MinValue);
            Assert.AreEqual(home.LatestOrder, DateTime.MinValue);
            Assert.AreEqual(home.OldestOrder, DateTime.MinValue);
            Assert.AreEqual(home.RecipesCount, 0);
            Assert.AreEqual(home.AdditivesCount, 0);
        }

        [Test]
        public void HomeNewOrder()
        {
            var home = new HomeViewModel(ordersService, recipesService, additivesService, abstractService);
            Assert.IsNotNull(home);

            ordersService.AddItemAsync(new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
            });

            home.RefreshCommand.Execute(null);

            Assert.AreEqual(home.OpenOrdersCount, 1);
            Assert.AreEqual(home.MixedOrdersCount, 0);
            Assert.AreEqual(home.RatedOrdersCount, 0);
            Assert.AreEqual(home.NextOrder, DateTime.Parse("2022-07-28T08:00:00"));
            Assert.AreEqual(home.LatestOrder, DateTime.Parse("2022-07-18T08:00:00"));
            Assert.AreEqual(home.OldestOrder, DateTime.Parse("2022-07-18T08:00:00"));
            Assert.AreEqual(home.RecipesCount, 0);
            Assert.AreEqual(home.AdditivesCount, 0);
        }

        [Test]
        public void HomeNewRecipe()
        {
            var home = new HomeViewModel(ordersService, recipesService, additivesService, abstractService);
            Assert.IsNotNull(home);

            recipesService.AddItemAsync(new Recipe
            {
                Id = "1",
                Additives = new List<Additive>()
            });

            home.RefreshCommand.Execute(null);

            Assert.AreEqual(home.OpenOrdersCount, 0);
            Assert.AreEqual(home.MixedOrdersCount, 0);
            Assert.AreEqual(home.RatedOrdersCount, 0);
            Assert.AreEqual(home.NextOrder, DateTime.MinValue);
            Assert.AreEqual(home.LatestOrder, DateTime.MinValue);
            Assert.AreEqual(home.OldestOrder, DateTime.MinValue);
            Assert.AreEqual(home.RecipesCount, 1);
            Assert.AreEqual(home.AdditivesCount, 0);
        }

        [Test]
        public void HomeNewAdditive()
        {
            var home = new HomeViewModel(ordersService, recipesService, additivesService, abstractService);
            Assert.IsNotNull(home);

            additivesService.AddItemAsync(new Additive
            {
                Id = "1",
                Name = "Test"
            });

            home.RefreshCommand.Execute(null);

            Assert.AreEqual(home.OpenOrdersCount, 0);
            Assert.AreEqual(home.MixedOrdersCount, 0);
            Assert.AreEqual(home.RatedOrdersCount, 0);
            Assert.AreEqual(home.NextOrder, DateTime.MinValue);
            Assert.AreEqual(home.LatestOrder, DateTime.MinValue);
            Assert.AreEqual(home.OldestOrder, DateTime.MinValue);
            Assert.AreEqual(home.RecipesCount, 0);
            Assert.AreEqual(home.AdditivesCount, 1);
        }
    }
}
