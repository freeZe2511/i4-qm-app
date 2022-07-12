using I4_QM_app.Models;
using I4_QM_app.NUnitTests.MockServices;
using I4_QM_app.Services.Data;
using I4_QM_app.Services.Notifications;
using I4_QM_app.ViewModels.History;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace I4_QM_app.NUnitTests.Tests.History
{
    internal class HistoryDetail
    {
        private IDataService<Order> ordersService;
        private INotificationService notificationsService;

        [SetUp]
        public void Setup()
        {
            ordersService = new MockOrderService();
            notificationsService = new MockNotificationsService();
        }

        [Test]
        public void HistoryDetailInit()
        {
            var history = new HistoryDetailViewModel(ordersService, notificationsService);
            Assert.IsNotNull(history.FeedbackCommand);
            Assert.IsNotNull(history.DeleteItemCommand);
        }

        [Test]
        public void HistoryDetailFeedback()
        {
            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                UserId = "1234",
                Status = Status.Mixed,
                Done = DateTime.Parse("2022-07-21T08:00:00"),
            };

            ordersService.AddItemAsync(a);

            var history = new HistoryDetailViewModel(ordersService, notificationsService);
            Assert.IsNotNull(history.FeedbackCommand);
            Assert.IsNotNull(history.DeleteItemCommand);

            history.OrderId = "1";
            Assert.IsTrue(history.FeedbackEnabled);
            history.FeedbackCommand.Execute(null);
        }

        [Test]
        public void HistoryDetailInitWithOrderID()
        {
            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                UserId = "1234",
                Status = Status.Mixed,
                Done = DateTime.Parse("2022-07-21T08:00:00"),
            };

            ordersService.AddItemAsync(a);

            var history = new HistoryDetailViewModel(ordersService, notificationsService);
            history.OrderId = "1";

            Assert.AreEqual(a.Id, history.OrderId);
            Assert.AreEqual(a.Id, history.Order.Id);
            Assert.AreEqual(a.Id, history.Id);
            Assert.AreEqual(a.UserId, history.UserId);
            Assert.AreEqual(a.Amount, history.Amount);
            Assert.AreEqual(a.Weight, history.Weight);
            Assert.AreEqual(a.Additives, history.Additives);
            Assert.AreEqual(a.Status, history.Status);
            Assert.AreEqual(a.Received, history.Received);
            Assert.AreEqual(a.Due, history.Due);
            Assert.AreEqual(a.Done, history.Done);
            Assert.AreEqual(a.Rating, history.Rating);
        }

        [Test]
        public async Task HistoryDetailDeleteAsync()
        {
            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                UserId = "1234",
                Status = Status.Mixed,
                Done = DateTime.Parse("2022-07-21T08:00:00"),
            };

            await ordersService.AddItemAsync(a);

            var history = new HistoryDetailViewModel(ordersService, notificationsService);
            history.OrderId = "1";

            history.DeleteItemCommand.Execute(null);

            Assert.IsEmpty(await ordersService.GetItemsAsync());
        }
    }
}
