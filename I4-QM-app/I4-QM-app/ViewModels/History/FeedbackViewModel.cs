using I4_QM_app.Models;
using I4_QM_app.Views;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    [QueryProperty(nameof(OrderId), nameof(OrderId))]
    public class FeedbackViewModel : BaseViewModel
    {
        private Order order;
        private string orderId;
        private Rating rating;

        public Command SendFeedbackCommand { get; }

        public Command ResetFeedbackCommand { get; }

        public Command CancelCommand { get; }

        public Command UpdateCommand { get; }

        public FeedbackViewModel()
        {
            Title = "Feedback (1 - 9)";
            SendFeedbackCommand = new Command(RateFeedbackAsync, Validate);
            ResetFeedbackCommand = new Command(ResetFeedback);
            CancelCommand = new Command(OnCancel);
            UpdateCommand = new Command(OnUpdate);
            rating = new Rating();
            rating.RatingId = Guid.NewGuid().ToString();

        }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private bool Validate()
        {
            return Rating.Form > 0 && Rating.Color > 0 && Rating.Ridge > 0 && Rating.Surface > 0 && Rating.Surface > 0
                && Rating.Bindings > 0 && Rating.Sprue > 0 && Rating.DropIn > 0 && Rating.Demolding > 0 &&
                Rating.AirInclusion > 0 && Rating.Overall > 0 && Rating.Feedback.Length > 0;
        }

        private void OnUpdate(object sender)
        {
            try
            {
                Console.WriteLine(sender);
                SendFeedbackCommand.ChangeCanExecute();

            }
            catch (Exception ex)
            {

            }

        }

        public Order Order
        {
            get => order;
            set => SetProperty(ref order, value);
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
        public Rating Rating
        {
            get => rating;
            set => SetProperty(ref rating, value);
        }

        public async void LoadOrderId(string orderId)
        {
            try
            {
                var order = await App.OrdersDataService.GetItemAsync(orderId);
                Order = order;

            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        private async void RateFeedbackAsync()
        {
            bool answer = await App.NotificationService.ShowSimpleDisplayAlert("Confirmation", "Send feedback?", "Yes", "No");

            // TODO parameter
            if (answer)
            {
                // update                
                Order.Status = Status.Rated;
                Order.Rating = Rating;
                await App.OrdersDataService.UpdateItemAsync(Order);

                // send mqtt
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                };

                string res = JsonSerializer.Serialize<Order>(Order, options);

                await App.ConnectionService.HandlePublishMessage("prod/orders/rated", res);

                await Shell.Current.GoToAsync($"//{nameof(HistoryPage)}");
            }
        }

        private void ResetFeedback()
        {
            Rating = new Rating();
            Rating.RatingId = Guid.NewGuid().ToString();
            Order.Rating = Rating;
        }
    }
}
