using I4_QM_app.Helpers;
using I4_QM_app.Models;
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
            // check if all additives are done (mock for enabled/disabled done btn)
            if (!Order.Additives.TrueForAll(a => a.Done == true)) return;

            // TODO display alert
            bool answer = await Shell.Current.DisplayAlert("Confirmation", "Done?", "Yes", "No");

            if (answer)
            {
                //calc new portions (percentages) -> should be dynamic with behavoir maybe
                foreach (var additive in Additives)
                {
                    additive.ActualPortion = (float)additive.Amount / (Weight * Amount / 100);
                }

                // update         
                Order.Status = Status.mixed;
                Order.Done = DateTime.Now;

                Console.WriteLine(Order.ToString());

                await App.OrdersDataStore.UpdateItemAsync(Order);

                // send mqtt
                await MqttConnection.HandleFinishedOrder(Order);

                // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
                await Shell.Current.GoToAsync($"//{nameof(OrdersPage)}");
            }


        }

        public async void LoadOrderId(string orderId)
        {
            try
            {
                var order = await App.OrdersDataStore.GetItemAsync(orderId);
                Order = order;
                Id = order.Id;

                //UserId = (string)Application.Current.Properties["UserID"];
                UserId = null;

                Amount = order.Amount;
                Weight = order.Weight;
                Additives = order.Additives;
                Status = order.Status;
                Created = order.Created;
                Due = order.Due;

                // calc
                foreach (var additive in Additives)
                {
                    additive.Amount = (int)(additive.Portion * Weight * Amount / 100);
                    //additive.Image = App.AdditiveDataSource. ...
                }


            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

    }

    // maybe todo in later iteration (fynamic change of percentage = portion)
    // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/behaviors/creating

    //public class NumericValidationBehavior : Behavior<Entry>
    //{
    //    public static readonly BindableProperty Portion = BindableProperty.Create("Portion", typeof(int),
    //                                    typeof(NumericValidationBehavior), null);

    //    protected override void OnAttachedTo(Entry entry)
    //    {
    //        entry.TextChanged += OnEntryTextChanged;
    //        base.OnAttachedTo(entry);
    //    }

    //    protected override void OnDetachingFrom(Entry entry)
    //    {
    //        entry.TextChanged -= OnEntryTextChanged;
    //        base.OnDetachingFrom(entry);
    //    }

    //    void OnEntryTextChanged(object sender, TextChangedEventArgs args)
    //    {
    //        double result;
    //        //bool isValid = double.TryParse(args.NewTextValue, out result);
    //        //((Entry)sender).TextColor = isValid ? Color.Default : Color.Red;
    //        //Console.WriteLine(result);

    //        //calc new percentage
    //        Console.WriteLine(Portion.ReturnType);


    //    }
    //}

}

