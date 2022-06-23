using I4_QM_app.Models;
using I4_QM_app.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xamarin.Essentials;
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
        private DateTime received;
        private DateTime due;
        private DateTime done;

        public Command DoneCommand { get; }


        public OrderDetailViewModel()
        {
            // execute/ canexecute? => canexecute if all additives are checked?
            DoneCommand = new Command(OnDoneClicked, Validate);
            // TODO done btn enable/disable
            this.PropertyChanged +=
                (_, __) => DoneCommand.ChangeCanExecute();
        }
        private bool Validate()
        {
            //TODO check if all additives are done            
            //return Additives.TrueForAll(a => a.Checked == true);
            return true;
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
        public DateTime Received
        {
            get => received;
            set => SetProperty(ref received, value);
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
            // check if all additives are checked (mock for enabled/disabled done btn)
            if (!Additives.TrueForAll(a => a.Checked == true)) return;

            bool answer = await App.NotificationService.ShowSimpleDisplayAlert("Confirmation", "Done?", "Yes", "No");

            if (answer)
            {
                //calc new portions (percentages) -> should be dynamic with behavoir maybe TODO
                foreach (var additive in Additives)
                {
                    additive.ActualPortion = (float)additive.Amount / (Weight * Amount / 100);
                }

                // update         
                Order.Status = Status.mixed;
                Order.Done = DateTime.Now;
                Order.UserId = UserId;

                await App.OrdersDataService.UpdateItemAsync(Order);

                // send mqtt
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                };

                string res = JsonSerializer.Serialize<Order>(Order, options);

                await App.ConnectionService.HandlePublishMessage("prod/orders/mixed", res);

                // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
                await Shell.Current.GoToAsync($"//{nameof(OrdersPage)}");
                //await Shell.Current.GoToAsync("..");
            }


        }

        private async Task LoadOrderId(string orderId)
        {
            try
            {
                var order = await App.OrdersDataService.GetItemAsync(orderId);
                Order = order;
                Id = order.Id;
                UserId = Preferences.Get("UserID", string.Empty);
                Amount = order.Amount;
                Weight = order.Weight;
                Additives = order.Additives;
                Status = order.Status;
                Received = order.Received;
                Due = order.Due;

                // calc
                foreach (var additive in Additives)
                {
                    // TODO
                    additive.Checked = false;
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

