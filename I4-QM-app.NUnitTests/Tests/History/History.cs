using I4_QM_app.Models;
using I4_QM_app.NUnitTests.MockServices;
using I4_QM_app.Services;
using I4_QM_app.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.Tests.History
{
    internal class History
    {
        private IDataService<Order> ordersService;
        private INotificationService notificationService;
        private IConnectionService connectionService;

        [SetUp]
        public void Setup()
        {
            ordersService = new MockOrderService();
            notificationService = new MockNotificationsService();
            connectionService = new MockConnectionService();
        }

        [Test]
        public void HistoryInit()
        {
            var history = new HistoryViewModel(ordersService, notificationService, connectionService);
            Assert.AreEqual(history.Title, "History");
            Assert.IsTrue(history.Descending);
            Assert.IsNotNull(history.History);

            history.OnAppearing();
            Assert.IsTrue(history.IsBusy);

            Assert.IsNotNull(history.DisableCommand);
        }

        [Test]
        public async Task HistoryEmpty()
        {
            var history = new HistoryViewModel(ordersService, notificationService, connectionService);
            Assert.IsEmpty(await ordersService.GetItemsAsync());
            Assert.IsEmpty(history.History);
        }

        [Test]
        public async Task HistoryAddData()
        {
            var history = new HistoryViewModel(ordersService, notificationService, connectionService);
            Assert.IsEmpty(await ordersService.GetItemsAsync());
            Assert.IsEmpty(history.History);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Mixed,
                Done = DateTime.Now
            };

            await ordersService.AddItemAsync(a);

            history.LoadHistoryCommand.Execute(null);
            Assert.AreEqual(1, history.History.Count);
        }

        [Test]
        public async Task HistorySortDataById()
        {
            var history = new HistoryViewModel(ordersService, notificationService, connectionService);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Mixed,
                Done = DateTime.Now
            };

            var b = new Order
            {
                Id = "2",
                Additives = new List<Additive>(),
                Amount = 6,
                Weight = 20,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-19T08:00:00"),
                Status = Status.Rated,
                Done = DateTime.Now.AddDays(1)
            };

            await ordersService.AddItemAsync(a);
            await ordersService.AddItemAsync(b);

            history.LoadHistoryCommand.Execute(null);
            Assert.AreEqual(2, history.History.Count);

            Assert.AreEqual(a.Id, history.History[0].Id);

            history.Descending = false;
            history.SortByCommand.Execute("Id");

            Assert.AreEqual(b.Id, history.History[0].Id);
        }

        [Test]
        public async Task HistorySortDataByStatus()
        {
            var history = new HistoryViewModel(ordersService, notificationService, connectionService);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Mixed,
                Done = DateTime.Now
            };

            var b = new Order
            {
                Id = "2",
                Additives = new List<Additive>(),
                Amount = 6,
                Weight = 20,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-19T08:00:00"),
                Status = Status.Rated,
                Done = DateTime.Now.AddDays(1)
            };

            await ordersService.AddItemAsync(a);
            await ordersService.AddItemAsync(b);

            history.LoadHistoryCommand.Execute(null);
            Assert.AreEqual(2, history.History.Count);

            Assert.AreEqual(a.Id, history.History[0].Id);

            history.Descending = false;
            history.SortByCommand.Execute("Status");

            Assert.AreEqual(b.Id, history.History[0].Id);
        }

        [Test]
        public async Task HistorySortDataByDone()
        {
            var history = new HistoryViewModel(ordersService, notificationService, connectionService);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Mixed,
                Done = DateTime.Now
            };

            var b = new Order
            {
                Id = "2",
                Additives = new List<Additive>(),
                Amount = 6,
                Weight = 20,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-19T08:00:00"),
                Status = Status.Rated,
                Done = DateTime.Now.AddDays(1)
            };

            await ordersService.AddItemAsync(a);
            await ordersService.AddItemAsync(b);

            history.LoadHistoryCommand.Execute(null);
            Assert.AreEqual(2, history.History.Count);

            Assert.AreEqual(a.Id, history.History[0].Id);

            history.Descending = false;
            history.SortByCommand.Execute("Done");

            Assert.AreEqual(b.Id, history.History[0].Id);
        }

        [Test]
        public async Task HistorySortDataByAmount()
        {
            var history = new HistoryViewModel(ordersService, notificationService, connectionService);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Mixed,
                Done = DateTime.Now
            };

            var b = new Order
            {
                Id = "2",
                Additives = new List<Additive>(),
                Amount = 6,
                Weight = 20,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-19T08:00:00"),
                Status = Status.Rated,
                Done = DateTime.Now.AddDays(1)
            };

            await ordersService.AddItemAsync(a);
            await ordersService.AddItemAsync(b);

            history.LoadHistoryCommand.Execute(null);
            Assert.AreEqual(2, history.History.Count);

            Assert.AreEqual(a.Id, history.History[0].Id);

            history.Descending = false;
            history.SortByCommand.Execute("Amount");

            Assert.AreEqual(b.Id, history.History[0].Id);
        }

        [Test]
        public async Task HistorySortDataByReceived()
        {
            var history = new HistoryViewModel(ordersService, notificationService, connectionService);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Mixed,
                Done = DateTime.Now
            };

            var b = new Order
            {
                Id = "2",
                Additives = new List<Additive>(),
                Amount = 6,
                Weight = 20,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-19T08:00:00"),
                Status = Status.Rated,
                Done = DateTime.Now.AddDays(1)
            };

            await ordersService.AddItemAsync(a);
            await ordersService.AddItemAsync(b);

            history.LoadHistoryCommand.Execute(null);
            Assert.AreEqual(2, history.History.Count);

            Assert.AreEqual(a.Id, history.History[0].Id);

            history.Descending = false;
            history.SortByCommand.Execute("Received");

            Assert.AreEqual(b.Id, history.History[0].Id);
        }

        [Test]
        public async Task HistoryDelete()
        {
            var history = new HistoryViewModel(ordersService, notificationService, connectionService);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Mixed,
                Done = DateTime.Now
            };

            var b = new Order
            {
                Id = "2",
                Additives = new List<Additive>(),
                Amount = 6,
                Weight = 20,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-19T08:00:00"),
                Status = Status.Rated,
                Done = DateTime.Now.AddDays(1)
            };

            await ordersService.AddItemAsync(a);
            await ordersService.AddItemAsync(b);

            history.LoadHistoryCommand.Execute(null);
            Assert.AreEqual(2, history.History.Count);

            history.DeleteAllItemsCommand.Execute(null);

            Assert.AreEqual(0, history.History.Count);

        }

        [Test]
        public async Task HistoryOrderTapped()
        {
            var history = new HistoryViewModel(ordersService, notificationService, connectionService);

            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                Status = Status.Mixed,
                Done = DateTime.Now
            };

            await ordersService.AddItemAsync(a);
            history.LoadHistoryCommand.Execute(null);
            Assert.AreEqual(1, history.History.Count);

            history.OrderTapped.Execute(a);
            history.SelectedOrder = a;
            Assert.AreEqual(a.Id, history.SelectedOrder.Id);
        }
    }
}
