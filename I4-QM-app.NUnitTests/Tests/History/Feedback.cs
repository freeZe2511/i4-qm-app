using I4_QM_app.Models;
using I4_QM_app.NUnitTests.MockServices;
using I4_QM_app.Services;
using I4_QM_app.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace I4_QM_app.NUnitTests.Tests.History
{
    internal class Feedback
    {
        private IDataService<Order> ordersService;
        private INotificationService notificationsService;
        private IConnectionService connectionService;

        [SetUp]
        public void Setup()
        {
            ordersService = new MockOrderService();
            notificationsService = new MockNotificationsService();
            connectionService = new MockConnectionService();
        }

        [Test]
        public void FeedbackInit()
        {
            var feedback = new FeedbackViewModel(ordersService, notificationsService, connectionService);
            Assert.AreEqual(feedback.Title, "Feedback (1 - 9)");
            Assert.NotNull(feedback.Rating);
            Assert.NotNull(feedback.Rating.RatingId);
            Assert.NotNull(feedback.CancelCommand);
            Assert.AreEqual(0, feedback.Rating.Color);
            Assert.AreEqual(0, feedback.Rating.Ridge);
            Assert.AreEqual(0, feedback.Rating.AirInclusion);
            Assert.IsFalse(feedback.SendFeedbackCommand.CanExecute(null));
        }

        [Test]
        public void FeedbackInitWithOrderID()
        {
            var a = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
            };

            ordersService.AddItemAsync(a);

            var feedback = new FeedbackViewModel(ordersService, notificationsService, connectionService);
            feedback.OrderId = "1";

            Assert.AreEqual(a.Id, feedback.OrderId);
            Assert.AreEqual(a.Id, feedback.Order.Id);
        }

        [Test]
        public void FeedbackReset()
        {
            var feedback = new FeedbackViewModel(ordersService, notificationsService, connectionService);
            feedback.Order = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
            };

            feedback.Rating.Ridge = 6;
            feedback.Rating.Color = 6;
            feedback.Rating.Feedback = "test";

            feedback.ResetFeedbackCommand.Execute(null);

            Assert.NotNull(feedback.Rating.RatingId);
            Assert.AreEqual(0, feedback.Rating.Ridge);
            Assert.AreEqual(0, feedback.Rating.Color);
            Assert.AreEqual(null, feedback.Rating.Feedback);

            feedback.CancelCommand.Execute(null);
        }

        [Test]
        public void FeedbackSend()
        {
            var feedback = new FeedbackViewModel(ordersService, notificationsService, connectionService);
            feedback.Order = new Order
            {
                Id = "1",
                Additives = new List<Additive>(),
                Amount = 3,
                Weight = 10,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
            };

            Assert.IsNull(feedback.Order.Rating);

            feedback.Rating.Ridge = 6;
            feedback.Rating.Color = 6;
            feedback.Rating.Surface = 6;
            feedback.Rating.Sprue = 6;
            feedback.Rating.Bindings = 6;
            feedback.Rating.AirInclusion = 6;
            feedback.Rating.Demolding = 6;
            feedback.Rating.DropIn = 6;
            feedback.Rating.Form = 6;
            feedback.Rating.Overall = 6;
            feedback.Rating.Feedback = "test";

            feedback.UpdateCommand.Execute(null);
            Assert.IsTrue(feedback.SendFeedbackCommand.CanExecute(null));

            feedback.SendFeedbackCommand.Execute(null);

            Assert.IsNotNull(feedback.Order.Rating);
            Assert.AreEqual(feedback.Order.Status, Status.Rated);
        }
    }
}
