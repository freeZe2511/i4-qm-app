using I4_QM_app.Models;
using I4_QM_app.Views;
using LiteDB;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
        private ObservableCollection<Additive> additives;
        private Status status;
        private DateTime received;
        private DateTime due;
        private DateTime done;

        public Command DoneCommand { get; }

        public Command CancelCommand { get; }

        public Command UpdateCommand { get; }

        public Command EntryCommand { get; }


        public OrderDetailViewModel()
        {
            additives = new ObservableCollection<Additive>();
            DoneCommand = new Command(OnDoneClicked, Validate);
            CancelCommand = new Command(OnCancel);
            UpdateCommand = new Command(OnUpdate);
            EntryCommand = new Command(OnCompleteEntry);
        }

        private void OnUpdate()
        {
            DoneCommand.ChangeCanExecute();
        }

        private bool Validate()
        {
            return !Additives.Any(i => !i.Checked);
        }

        private void OnCompleteEntry(object sender)
        {
            if (sender.GetType() == typeof(Additive))
            {
                var newItem = (Additive)sender;
                newItem.ActualPortion = Math.Round(newItem.Amount / (Weight * Amount * 0.01), 2);
                var oldItem = Additives.FirstOrDefault(i => i.Id == newItem.Id);
                var oldIndex = Additives.IndexOf(oldItem);
                Additives[oldIndex] = newItem;
            }
        }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
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

        public ObservableCollection<Additive> Additives
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
            // code-side check if all additives are checked
            if (Additives.Any(a => !a.Checked))
            {
                return;
            }

            bool answer = await App.NotificationService.ShowSimpleDisplayAlert("Confirmation", "Done?", "Yes", "No");

            if (answer)
            {
                //calc new portions (percentages) -> should be dynamic with behavoir maybe TODO
                foreach (var additive in Additives)
                {
                    additive.ActualPortion = Math.Round(additive.Amount / (Weight * Amount * 0.01), 2);
                    additive.Image = null;
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

                string res = System.Text.Json.JsonSerializer.Serialize<Order>(Order, options);

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
                var additives = await App.AdditivesDataService.GetItemsAsync();

                Order = order;
                Id = order.Id;
                UserId = Preferences.Get("UserID", string.Empty);
                Amount = order.Amount;
                Weight = order.Weight;
                Status = order.Status;
                Received = order.Received;
                Due = order.Due;

                // calc
                foreach (var additive in order.Additives)
                {
                    additive.Checked = false;
                    additive.Amount = Math.Round(additive.Portion * Weight * Amount * 0.01, 2, MidpointRounding.AwayFromZero);
                    additive.ActualPortion = additive.Portion;

                    Additive item = additives.FirstOrDefault(x => x.Id == additive.Id);

                    if (item == null)
                    {
                        additive.Available = false;
                        additive.Image = ImageSource.FromFile("no_image.png");
                        continue;
                    }

                    additive.Available = true;

                    var fs = App.DB.GetStorage<string>("myImages");
                    LiteFileInfo<string> file = fs.FindById(additive.Id);

                    if (file != null)
                    {
                        additive.Image = ImageSource.FromStream(() => file.OpenRead());
                    }
                    else
                    {
                        additive.Image = ImageSource.FromFile("no_image.png");
                    }

                    Additives.Add(additive);
                }

                OnUpdate();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

    }

}

