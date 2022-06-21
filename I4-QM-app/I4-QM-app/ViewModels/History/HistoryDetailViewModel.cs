﻿using I4_QM_app.Models;
using I4_QM_app.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    [QueryProperty(nameof(OrderId), nameof(OrderId))]
    public class HistoryDetailViewModel : BaseViewModel
    {
        private Order order;
        private string orderId;
        private string id;
        private string userId;
        private int amount;
        private int weight;
        private List<Additive> additives;
        private Status status;
        private DateTime created;
        private DateTime due;
        private DateTime done;
        private Rating rating;
        private bool feedbackEnabled;

        public Command FeedbackCommand { get; }
        public Command DeleteItemCommand { get; }

        public HistoryDetailViewModel()
        {
            FeedbackCommand = new Command(OnFeedbackClicked);
            DeleteItemCommand = new Command(DeleteItem);
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

        public int Weight
        {
            get => weight;
            set => SetProperty(ref weight, value);
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

        public DateTime Done
        {
            get => done;
            set => SetProperty(ref done, value);
        }

        public Rating Rating
        {
            get => rating;
            set => SetProperty(ref rating, value);
        }

        public bool FeedbackEnabled
        {
            get => feedbackEnabled;
            set => SetProperty(ref feedbackEnabled, value);
        }

        private async void OnFeedbackClicked()
        {
            ////update
            //Order.Status = Status.rated;
            //await App.OrdersDataStore.UpdateItemAsync(Order);

            //// send mqtt
            ////await MqttConnection.HandleFinishedOrder(Order);

            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"{nameof(FeedbackPage)}?{nameof(FeedbackViewModel.OrderId)}={order.Id}");
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
                Weight = order.Weight;
                Additives = order.Additives;
                Status = order.Status;
                Created = order.Created;
                Due = order.Due;
                Done = order.Done;
                Rating = order.Rating;
                FeedbackEnabled = order.Status == Status.mixed;

            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        async void DeleteItem()
        {
            // TODO abstract dialog_service
            bool answer = await Shell.Current.DisplayAlert("Confirmation", "Delete item from history?", "Yes", "No");

            // TODO parameter
            if (answer)
            {
                await App.OrdersDataStore.DeleteItemAsync(Id);
                await Shell.Current.GoToAsync($"//{nameof(HistoryPage)}");
            }

        }
    }
}