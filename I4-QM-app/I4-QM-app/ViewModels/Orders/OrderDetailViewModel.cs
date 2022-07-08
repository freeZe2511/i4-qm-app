using I4_QM_app.Models;
using I4_QM_app.Services;
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
    /// <summary>
    /// ViewModel for Orders DetailPage.
    /// </summary>
    [QueryProperty(nameof(OrderId), nameof(OrderId))]
    public class OrderDetailViewModel : BaseViewModel
    {
        private readonly IDataService<Order> ordersService;
        private readonly INotificationService notificationService;
        private readonly IConnectionService connectionService;
        private readonly IDataService<Additive> additivesService;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetailViewModel"/> class.
        /// </summary>
        /// <param name="ordersService">Orders Service.</param>
        /// <param name="notificationService">Notifications Service.</param>
        /// <param name="connectionService">Connection Service.</param>
        /// <param name="additivesService">Additives Service.</param>
        public OrderDetailViewModel(IDataService<Order> ordersService, INotificationService notificationService, IConnectionService connectionService, IDataService<Additive> additivesService)
        {
            this.ordersService = ordersService;
            this.notificationService = notificationService;
            this.connectionService = connectionService;
            this.additivesService = additivesService;

            additives = new ObservableCollection<Additive>();
            DoneCommand = new Command(OnDoneClicked, Validate);
            CancelCommand = new Command(OnCancel);
            UpdateCommand = new Command(OnUpdate);
            EntryCommand = new Command(OnCompleteEntry);
            RefreshCommand = new Command(async () => await LoadOrderId(OrderId));
        }

        /// <summary>
        /// Gets command to finish mixing.
        /// </summary>
        public Command DoneCommand { get; }

        /// <summary>
        /// Gets command to cancel mixing.
        /// </summary>
        public Command CancelCommand { get; }

        /// <summary>
        /// Gets command to update UI.
        /// </summary>
        public Command UpdateCommand { get; }

        /// <summary>
        /// Gets command to update on entry.
        /// </summary>
        public Command EntryCommand { get; }

        /// <summary>
        /// Gets command to refresh additive list in case of changes.
        /// </summary>
        public Command RefreshCommand { get; }

        /// <summary>
        /// Gets or sets the order id.
        /// </summary>
        public string OrderId
        {
            get => orderId;
            set
            {
                orderId = value;
                LoadOrderId(value);
            }
        }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public Order Order
        {
            get => order;
            set => SetProperty(ref order, value);
        }

        /// <summary>
        /// Gets or sets the order id.
        /// </summary>
        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string UserId
        {
            get => userId;
            set => SetProperty(ref userId, value);
        }

        /// <summary>
        /// Gets or sets the total order amount.
        /// </summary>
        public int Amount
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }

        /// <summary>
        /// Gets or sets the order single item weight.
        /// </summary>
        public int Weight
        {
            get => weight;
            set => SetProperty(ref weight, value);
        }

        /// <summary>
        /// Gets or sets the order additives list.
        /// </summary>
        public ObservableCollection<Additive> Additives
        {
            get => additives;
            set => SetProperty(ref additives, value);
        }

        /// <summary>
        /// Gets or sets the order status.
        /// </summary>
        public Status Status
        {
            get => status;
            set => SetProperty(ref status, value);
        }

        /// <summary>
        /// Gets or sets the order received date.
        /// </summary>
        public DateTime Received
        {
            get => received;
            set => SetProperty(ref received, value);
        }

        /// <summary>
        /// Gets or sets the order due date.
        /// </summary>
        public DateTime Due
        {
            get => due;
            set => SetProperty(ref due, value);
        }

        /// <summary>
        /// Gets or sets the order done date.
        /// </summary>
        public DateTime Done
        {
            get => done;
            set => SetProperty(ref done, value);
        }

        /// <summary>
        /// Updates order and sends mixed order to server and db.
        /// </summary>
        private async void OnDoneClicked()
        {
            // code-side check if all additives are checked
            if (Additives.Any(a => !a.Checked))
            {
                return;
            }

            bool answer = await notificationService.ShowSimpleDisplayAlert("Confirmation", "Done?", "Yes", "No");

            if (answer)
            {
                foreach (var additive in Additives)
                {
                    additive.ActualPortion = Math.Round(additive.Amount / (Weight * Amount * 0.01), 2);
                    additive.Image = null;
                }

                // update
                Order.Status = Status.Mixed;
                Order.Done = DateTime.Now;
                Order.UserId = UserId;

                await ordersService.UpdateItemAsync(Order);

                // send mqtt
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                };

                string res = System.Text.Json.JsonSerializer.Serialize<Order>(Order, options);

                await connectionService.HandlePublishMessage("prod/orders/mixed", res);
                await Shell.Current.GoToAsync($"//{nameof(OrdersPage)}");
            }
        }

        /// <summary>
        /// Load order with additives from db, calculates amount and set additive image.
        /// </summary>
        /// <param name="orderId">Order Id.</param>
        /// <returns>Task.</returns>
        private async Task LoadOrderId(string orderId)
        {
            IsBusy = true;

            try
            {
                var orderTemp = await ordersService.GetItemAsync(orderId);
                var additivesTemp = await additivesService.GetItemsAsync();
                var fs = App.DB.GetStorage<string>("myImages");

                Order = orderTemp;
                Id = orderTemp.Id;
                UserId = Preferences.Get("UserID", string.Empty);
                Amount = orderTemp.Amount;
                Weight = orderTemp.Weight;
                Status = orderTemp.Status;
                Received = orderTemp.Received;
                Due = orderTemp.Due;

                Additives.Clear();

                foreach (var additive in orderTemp.Additives)
                {
                    Additives.Add(additive);

                    additive.Checked = false;
                    additive.Amount = Math.Round(additive.Portion * Weight * Amount * 0.01, 2, MidpointRounding.AwayFromZero);
                    additive.ActualPortion = additive.Portion;

                    Additive item = additivesTemp.FirstOrDefault(x => x.Id == additive.Id);

                    if (item == null)
                    {
                        additive.Available = false;
                        additive.Image = ImageSource.FromFile("no_image.png");
                        continue;
                    }

                    additive.Available = true;
                    additive.Name = item.Name;
                    additive.Image = GetAdditiveImage(fs, additive.Id);
                }

                OnUpdate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Returns saved image for additive in filestorage or no_image.
        /// </summary>
        /// <param name="fs">ILiteStorage.</param>
        /// <param name="id">Id.</param>
        /// <returns>ImageSource.</returns>
        private ImageSource GetAdditiveImage(ILiteStorage<string> fs, string id)
        {
            LiteFileInfo<string> file = fs.FindById(id);

            if (file != null)
            {
                return ImageSource.FromStream(() => file.OpenRead());
            }
            else
            {
                return ImageSource.FromFile("no_image.png");
            }
        }

        /// <summary>
        /// Update UI.
        /// </summary>
        private void OnUpdate()
        {
            DoneCommand.ChangeCanExecute();
        }

        /// <summary>
        /// Returns if form is valid (all additives checked).
        /// </summary>
        /// <returns>bool.</returns>
        private bool Validate()
        {
            return Additives.Count > 0 && !Additives.Any(i => !i.Checked);
        }

        /// <summary>
        /// Calculates new actualPortion when entry completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
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

        /// <summary>
        /// Navigates back.
        /// </summary>
        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}