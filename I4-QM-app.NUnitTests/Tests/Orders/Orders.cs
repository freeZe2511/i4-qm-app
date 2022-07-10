using I4_QM_app.Models;
using I4_QM_app.NUnitTests.MockServices;
using I4_QM_app.Services;
using I4_QM_app.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.Tests.Orders
{
    internal class Orders
    {
        private IDataService<Order> ordersService;
        private INotificationService notificationService;

        [SetUp]
        public void Setup()
        {
            ordersService = new MockOrderService();
            notificationService = new MockNotificationsService();
        }

        [Test]
        public void OrdersInit()
        {
            var orders = new OrdersViewModel(ordersService, notificationService);
            Assert.AreEqual(orders.Title, "Orders");
            Assert.IsTrue(orders.Descending);
            Assert.IsNotNull(orders.Orders);

            orders.OnAppearing();
            Assert.IsTrue(orders.IsBusy);

            Assert.IsNotNull(orders.DisableCommand);
        }

        [Test]
        public async Task OrdersEmpty()
        {
            var orders = new OrdersViewModel(ordersService, notificationService);
            Assert.IsEmpty(await ordersService.GetItemsAsync());
            Assert.IsEmpty(orders.Orders);
        }

        [Test]
        public async Task OrdersAddData()
        {
            var orders = new OrdersViewModel(ordersService, notificationService);
            Assert.IsEmpty(await ordersService.GetItemsAsync());
            Assert.IsEmpty(orders.Orders);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Open,
            };

            await ordersService.AddItemAsync(a);

            orders.LoadOrdersCommand.Execute(null);
            Assert.AreEqual(1, orders.Orders.Count);
        }

        [Test]
        public async Task OrdersSortDataById()
        {
            var orders = new OrdersViewModel(ordersService, notificationService);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Open,
            };

            var b = new Order
            {
                Id = "2",
                Additives = new List<Additive>(),
                Amount = 6,
                Weight = 20,
                Due = DateTime.Parse("2022-07-29T08:00:00"),
                Received = DateTime.Parse("2022-07-19T08:00:00"),
                Status = Status.Open,
            };

            await ordersService.AddItemAsync(a);
            await ordersService.AddItemAsync(b);

            orders.LoadOrdersCommand.Execute(null);
            Assert.AreEqual(2, orders.Orders.Count);

            Assert.AreEqual(a.Id, orders.Orders[0].Id);

            orders.Descending = false;
            orders.SortByCommand.Execute("Id");

            Assert.AreEqual(b.Id, orders.Orders[0].Id);
        }

        [Test]
        public async Task OrdersSortDataByDue()
        {
            var orders = new OrdersViewModel(ordersService, notificationService);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Open,
            };

            var b = new Order
            {
                Id = "2",
                Additives = new List<Additive>(),
                Amount = 6,
                Weight = 20,
                Due = DateTime.Parse("2022-07-29T08:00:00"),
                Received = DateTime.Parse("2022-07-19T08:00:00"),
                Status = Status.Open,
            };

            await ordersService.AddItemAsync(a);
            await ordersService.AddItemAsync(b);

            orders.LoadOrdersCommand.Execute(null);
            Assert.AreEqual(2, orders.Orders.Count);

            Assert.AreEqual(a.Id, orders.Orders[0].Id);

            orders.Descending = false;
            orders.SortByCommand.Execute("Due");

            Assert.AreEqual(b.Id, orders.Orders[0].Id);
        }

        [Test]
        public async Task OrdersSortDataByAmount()
        {
            var orders = new OrdersViewModel(ordersService, notificationService);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Open,
            };

            var b = new Order
            {
                Id = "2",
                Additives = new List<Additive>(),
                Amount = 6,
                Weight = 20,
                Due = DateTime.Parse("2022-07-29T08:00:00"),
                Received = DateTime.Parse("2022-07-19T08:00:00"),
                Status = Status.Open,
            };

            await ordersService.AddItemAsync(a);
            await ordersService.AddItemAsync(b);

            orders.LoadOrdersCommand.Execute(null);
            Assert.AreEqual(2, orders.Orders.Count);

            Assert.AreEqual(a.Id, orders.Orders[0].Id);

            orders.Descending = false;
            orders.SortByCommand.Execute("Amount");

            Assert.AreEqual(b.Id, orders.Orders[0].Id);
        }

        [Test]
        public async Task OrdersSortDataByReceived()
        {
            var orders = new OrdersViewModel(ordersService, notificationService);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Open,
            };

            var b = new Order
            {
                Id = "2",
                Additives = new List<Additive>(),
                Amount = 6,
                Weight = 20,
                Due = DateTime.Parse("2022-07-29T08:00:00"),
                Received = DateTime.Parse("2022-07-19T08:00:00"),
                Status = Status.Open,
            };

            await ordersService.AddItemAsync(a);
            await ordersService.AddItemAsync(b);

            orders.LoadOrdersCommand.Execute(null);
            Assert.AreEqual(2, orders.Orders.Count);

            Assert.AreEqual(a.Id, orders.Orders[0].Id);

            orders.Descending = false;
            orders.SortByCommand.Execute("Received");

            Assert.AreEqual(b.Id, orders.Orders[0].Id);
        }

        [Test]
        public async Task OrdersOrderTapped()
        {
            var orders = new OrdersViewModel(ordersService, notificationService);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Open,
            };

            await ordersService.AddItemAsync(a);
            orders.LoadOrdersCommand.Execute(null);
            Assert.AreEqual(1, orders.Orders.Count);

            orders.OrderTapped.Execute(a);
            orders.SelectedOrder = a;
            Assert.AreEqual(a.Id, orders.SelectedOrder.Id);
        }
    }
}
