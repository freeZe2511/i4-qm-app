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

        private bool doneEnabled;

        public Command DoneCommand { get; }

        public bool DoneEnabled
        {
            get => doneEnabled;
            set { doneEnabled = value; }
        }

        public OrderDetailViewModel()
        {
            // execute/ canexecute? => canexecute if all additives are checked?
            DoneCommand = new Command(OnDoneClicked);
            // TODO done btn enable/disable
            doneEnabled = true;
        }

        public string OrderId
        {
            get => orderId;
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

        private async void OnDoneClicked()
        {
            // check if all additives are done (mock for enabled/disabled done btn)
            if (!Order.Additives.TrueForAll(a => a.Done == true)) return;

            // set done and save in history         
            Order.Status = Status.done;
            // needed?! or just delete entry in orders db
            await App.OrdersDataStore.UpdateItemAsync(Order);
            await App.HistoryDataStore.AddItemAsync(Order);

            // send mqtt
            await MqttConnection.HandleFinishedOrder(Order);

            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(OrdersPage)}");
        }

        public async void LoadOrderId(string orderId)
        {
            try
            {
                var order = await App.OrdersDataStore.GetItemAsync(orderId);
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

