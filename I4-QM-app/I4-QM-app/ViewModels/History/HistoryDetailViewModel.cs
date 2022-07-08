using I4_QM_app.Models;
using I4_QM_app.Services;
using I4_QM_app.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    /// <summary>
    /// ViewModel for History DetailsPage.
    /// </summary>
    [QueryProperty(nameof(OrderId), nameof(OrderId))]
    public class HistoryDetailViewModel : BaseViewModel
    {
        private readonly IDataService<Order> ordersService;
        private readonly INotificationService notificationService;

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
        private Rating rating;
        private bool feedbackEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryDetailViewModel"/> class.
        /// </summary>
        /// <param name="ordersService">Orders Service.</param>
        /// <param name="notificationService">Notifications Service.</param>
        public HistoryDetailViewModel(IDataService<Order> ordersService, INotificationService notificationService)
        {
            this.ordersService = ordersService;
            this.notificationService = notificationService;
            FeedbackCommand = new Command(OnFeedbackClicked);
            DeleteItemCommand = new Command(async () => await DeleteItem());
        }

        /// <summary>
        /// Gets command to go to feedbackPage.
        /// </summary>
        public Command FeedbackCommand { get; }

        /// <summary>
        /// Gets command to delete history item.
        /// </summary>
        public Command DeleteItemCommand { get; }

        /// <summary>
        /// Gets or sets order id and initially loads from db.
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
        /// Gets or sets the order userid.
        /// </summary>
        public string UserId
        {
            get => userId;
            set => SetProperty(ref userId, value);
        }

        /// <summary>
        /// Gets or sets the order amount.
        /// </summary>
        public int Amount
        {
            get => amount;
            set => SetProperty(ref amount, value);
        }

        /// <summary>
        /// Gets or sets the order weight.
        /// </summary>
        public int Weight
        {
            get => weight;
            set => SetProperty(ref weight, value);
        }

        /// <summary>
        /// Gets or sets the order additives.
        /// </summary>
        public List<Additive> Additives
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
        /// Gets or sets the order rating.
        /// </summary>
        public Rating Rating
        {
            get => rating;
            set => SetProperty(ref rating, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether feedback is enabled.
        /// </summary>
        public bool FeedbackEnabled
        {
            get => feedbackEnabled;
            set => SetProperty(ref feedbackEnabled, value);
        }

        /// <summary>
        /// Navigate to FeedbackPage.
        /// </summary>
        private async void OnFeedbackClicked()
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"{nameof(FeedbackPage)}?{nameof(FeedbackViewModel.OrderId)}={order.Id}");
        }

        /// <summary>
        /// Load order from db.
        /// </summary>
        /// <param name="orderId">OrderId.</param>
        /// <returns>Task.</returns>
        private async Task LoadOrderId(string orderId)
        {
            try
            {
                var orderTemp = await ordersService.GetItemAsync(orderId);
                Order = orderTemp;
                Id = orderTemp.Id;
                UserId = orderTemp.UserId;
                Amount = orderTemp.Amount;
                Weight = orderTemp.Weight;
                Additives = orderTemp.Additives;
                Status = orderTemp.Status;
                Received = orderTemp.Received;
                Due = orderTemp.Due;
                Done = orderTemp.Done;
                Rating = orderTemp.Rating;
                FeedbackEnabled = orderTemp.Status == Status.Mixed;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        /// <summary>
        /// Delete history item after confirmation.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task DeleteItem()
        {
            bool answer = await notificationService.ShowSimpleDisplayAlert("Confirmation", "Delete item from history?", "Yes", "No");

            if (answer)
            {
                await ordersService.DeleteItemAsync(Id);
                await Shell.Current.GoToAsync($"//{nameof(HistoryPage)}");
            }
        }
    }
}
