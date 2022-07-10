using I4_QM_app.Models;
using I4_QM_app.NUnitTests.MockServices;
using I4_QM_app.Services;
using I4_QM_app.Services.Abstract;
using I4_QM_app.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace I4_QM_app.NUnitTests.Tests.Orders
{
    internal class OrdersDetail
    {
        private IDataService<Order> ordersService;
        private INotificationService notificationsService;
        private IConnectionService connectionService;
        private IDataService<Additive> additivesService;
        private IAbstractService abstractService;

        [SetUp]
        public void Setup()
        {
            ordersService = new MockOrderService();
            notificationsService = new MockNotificationsService();
            connectionService = new MockConnectionService();
            additivesService = new MockAdditivesService();
            abstractService = new MockAbstractService();
        }

        [Test]
        public void OrdersDetailInit()
        {
            var orders = new OrderDetailViewModel(ordersService, notificationsService, connectionService, additivesService, abstractService);
            Assert.IsNotNull(orders.CancelCommand);
            Assert.IsNotNull(orders.UpdateCommand);

            Assert.IsFalse(orders.DoneCommand.CanExecute(null));

            orders.CancelCommand.Execute(null);
        }

        [Test]
        public void OrdersDetailInitWithOrderID()
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
                Status = Status.Open,
            };

            ordersService.AddItemAsync(a);
            abstractService.SetPreferences("UserID", "1234");

            var orders = new OrderDetailViewModel(ordersService, notificationsService, connectionService, additivesService, abstractService);
            orders.OrderId = "1";

            Assert.AreEqual(a.Id, orders.OrderId);
            Assert.AreEqual(a.Id, orders.Order.Id);
            Assert.AreEqual(a.Id, orders.Id);
            Assert.AreEqual(a.UserId, orders.UserId);
            Assert.AreEqual(a.Amount, orders.Amount);
            Assert.AreEqual(a.Weight, orders.Weight);
            Assert.AreEqual(a.Additives, orders.Additives);
            Assert.AreEqual(a.Status, orders.Status);
            Assert.AreEqual(a.Received, orders.Received);
            Assert.AreEqual(a.Due, orders.Due);
            Assert.AreEqual(a.Done, orders.Done);
        }

        [Test]
        public void OrdersDetailDone()
        {
            var additives = new List<Additive>();

            var a = new Additive
            {
                Id = "1",
                Portion = 5,
                Name = "test1"
            };

            var b = new Additive
            {
                Id = "2",
                Portion = 100,
                Name = "test2"
            };

            additives.Add(a);
            additives.Add(b);
            additivesService.AddItemAsync(a);

            var c = new Order
            {
                Id = "1",
                Additives = additives,
                Amount = 10,
                Weight = 3,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                UserId = "1234",
                Status = Status.Open,
            };

            ordersService.AddItemAsync(c);
            abstractService.SetPreferences("UserID", "1234");

            var orders = new OrderDetailViewModel(ordersService, notificationsService, connectionService, additivesService, abstractService);
            orders.OrderId = "1";

            Assert.IsFalse(orders.DoneCommand.CanExecute(null));

            foreach (var additive in orders.Additives)
            {
                additive.Checked = true;
            }

            Assert.IsTrue(orders.DoneCommand.CanExecute(null));
            orders.DoneCommand.Execute(null);
        }

        [Test]
        public void OrdersDetailDoneEntry()
        {
            var additives = new List<Additive>();

            var a = new Additive
            {
                Id = "1",
                Portion = 5,
                Name = "test1"
            };

            var b = new Additive
            {
                Id = "2",
                Portion = 100,
                Name = "test2"
            };

            additives.Add(a);
            additives.Add(b);
            additivesService.AddItemAsync(a);

            var c = new Order
            {
                Id = "1",
                Additives = additives,
                Amount = 10,
                Weight = 3,
                Due = DateTime.Parse("2022-07-28T08:00:00"),
                Received = DateTime.Parse("2022-07-18T08:00:00"),
                UserId = "1234",
                Status = Status.Open,
            };

            ordersService.AddItemAsync(c);
            abstractService.SetPreferences("UserID", "1234");

            var orders = new OrderDetailViewModel(ordersService, notificationsService, connectionService, additivesService, abstractService);
            orders.OrderId = "1";

            orders.RefreshCommand.Execute(null);

            Assert.IsFalse(orders.DoneCommand.CanExecute(null));

            foreach (var additive in orders.Additives)
            {
                additive.Checked = true;
            }

            Assert.IsTrue(orders.DoneCommand.CanExecute(null));

            orders.EntryCommand.Execute(additives[0]);
            Assert.AreEqual(5, orders.Additives[0].ActualPortion);

            orders.Additives[0].Amount = 3;
            orders.EntryCommand.Execute(additives[0]);
            Assert.AreEqual(10, orders.Additives[0].ActualPortion);
        }
    }
}
