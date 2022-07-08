using I4_QM_app.Models;
using I4_QM_app.Services;
using I4_QM_app.Views;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    /// <summary>
    /// ViewModel for Feedback Page.
    /// </summary>
    [QueryProperty(nameof(OrderId), nameof(OrderId))]
    public class FeedbackViewModel : BaseViewModel
    {
        private readonly IDataService<Order> ordersService;
        private readonly INotificationService notificationsService;
        private readonly IConnectionService connectionService;

        private Order order;
        private string orderId;
        private Rating rating;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackViewModel"/> class.
        /// </summary>
        /// <param name="ordersService">Orders Service.</param>
        /// <param name="notificationsService">Notifications Service.</param>
        /// <param name="connectionService">Connection Service.</param>
        public FeedbackViewModel(IDataService<Order> ordersService, INotificationService notificationsService, IConnectionService connectionService)
        {
            this.ordersService = ordersService;
            this.notificationsService = notificationsService;
            this.connectionService = connectionService;

            Title = "Feedback (1 - 9)";
            SendFeedbackCommand = new Command(async () => await RateFeedbackAsync(), Validate);
            ResetFeedbackCommand = new Command(ResetFeedback);
            CancelCommand = new Command(OnCancel);
            UpdateCommand = new Command(OnUpdate);
            rating = new Rating();
            rating.RatingId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets command to send feedback.
        /// </summary>
        public Command SendFeedbackCommand { get; }

        /// <summary>
        /// Gets command to clear feedback form.
        /// </summary>
        public Command ResetFeedbackCommand { get; }

        /// <summary>
        /// Gets command to cancel feedback process.
        /// </summary>
        public Command CancelCommand { get; }

        /// <summary>
        /// Gets command to update UI.
        /// </summary>
        public Command UpdateCommand { get; }

        /// <summary>
        /// Gets or sets the order to feedback.
        /// </summary>
        public Order Order
        {
            get => order;
            set => SetProperty(ref order, value);
        }

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
        /// Gets or sets the order rating.
        /// </summary>
        public Rating Rating
        {
            get => rating;
            set => SetProperty(ref rating, value);
        }

        /// <summary>
        /// Load order from db with id.
        /// </summary>
        /// <param name="orderId">orderId.</param>
        /// <returns>Task.</returns>
        public async Task LoadOrderId(string orderId)
        {
            try
            {
                Order = await ordersService.GetItemAsync(orderId);
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        /// <summary>
        /// Send rating to server after confirmation.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task RateFeedbackAsync()
        {
            bool answer = await notificationsService.ShowSimpleDisplayAlert("Confirmation", "Send feedback?", "Yes", "No");

            if (answer)
            {
                // update
                Order.Status = Status.Rated;
                Order.Rating = Rating;
                await ordersService.UpdateItemAsync(Order);

                // send mqtt
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                };

                string res = JsonSerializer.Serialize(Order, options);

                await connectionService.HandlePublishMessage("prod/orders/rated", res);

                await Shell.Current.GoToAsync($"//{nameof(HistoryPage)}");
            }
        }

        /// <summary>
        /// Reset feedback form.
        /// </summary>
        private void ResetFeedback()
        {
            Rating = new Rating
            {
                RatingId = Guid.NewGuid().ToString(),
            };
            Order.Rating = Rating;
        }

        /// <summary>
        /// Navigate back.
        /// </summary>
        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        /// <summary>
        /// Validate feedback form.
        /// </summary>
        /// <returns>bool.</returns>
        private bool Validate()
        {
            return Rating.Form > 0 && Rating.Color > 0 && Rating.Ridge > 0 && Rating.Surface > 0 && Rating.Surface > 0
                && Rating.Bindings > 0 && Rating.Sprue > 0 && Rating.DropIn > 0 && Rating.Demolding > 0 &&
                Rating.AirInclusion > 0 && Rating.Overall > 0 && Rating.Feedback.Length > 0;
        }

        /// <summary>
        /// Called to update UI.
        /// </summary>
        /// <param name="sender">Sender.</param>
        private void OnUpdate(object sender)
        {
            try
            {
                Console.WriteLine(sender);
                SendFeedbackCommand.ChangeCanExecute();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
