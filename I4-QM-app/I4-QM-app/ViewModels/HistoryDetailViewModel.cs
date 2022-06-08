﻿using I4_QM_app.Models;
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

        private bool doneEnabled;

        public Command DoneCommand { get; }

        public bool DoneEnabled
        {
            get => doneEnabled;
            set { doneEnabled = value; }
        }

        public HistoryDetailViewModel()
        {

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

        private async void OnDoneClicked()
        {

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

            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}