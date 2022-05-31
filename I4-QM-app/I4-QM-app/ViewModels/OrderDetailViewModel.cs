using I4_QM_app.Models;
using I4_QM_app.Services;
using I4_QM_app.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    [QueryProperty(nameof(OrderId), nameof(OrderId))]
    public class OrderDetailViewModel : BaseViewModel
    {
        private Order order;
        private string orderId;
        private string id;
        private string userId;
        private int amount;
        private List<Additive> additives;
        private Status status;
        private DateTime created;
        private DateTime due;

        public Command DoneCommand { get; }

        public OrderDetailViewModel()
        {
            // execute/ canexecute? => canexecute if all additives are checked?
            DoneCommand = new Command(OnDoneClicked);
        }

        public string OrderId
        {
            get
            {
                return orderId;
            }
            set
            {
                orderId = value;
                LoadOrderId(value);
            }
        }

        public Order Order
        {
            get => order;
            set => SetProperty(ref order, value);
        }

        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public string UserId
        {
            get => userId;
            set => SetProperty(ref userId, value);
        }
        public int Amount
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }
        public List<Additive> Additives
        {
            get => additives;
            set => SetProperty(ref additives, value);
        }
        public Status Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }
        public DateTime Created
        {
            get => created;
            set => SetProperty(ref created, value);
        }
        public DateTime Due
        {
            get => due;
            set => SetProperty(ref due, value);
        }

        private async void OnDoneClicked(object obj)
        {
            // TODO send mqtt
            await MqttConnection.Send_Message(Order);

            // test
            Console.WriteLine("test");
            await MqttConnection.Handle_Received_Application_Message();

            // test
            var order = await OrdersDataStore.GetItemAsync(Order.Id);
            order.Status = Status.waiting;
            await OrdersDataStore.UpdateItemAsync(order);

            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(OrdersPage)}");
        }

        public async void LoadOrderId(string orderId)
        {
            try
            {
                var order = await OrdersDataStore.GetItemAsync(orderId);
                Order = order;
                Id = order.Id;
                UserId = order.UserId;
                Amount = order.Amount;
                Additives = order.Additives;
                Status = order.Status;
                Created = order.Created;
                Due = order.Due;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

    }

}

