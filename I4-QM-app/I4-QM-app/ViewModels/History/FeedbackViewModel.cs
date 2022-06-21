using I4_QM_app.Models;
using I4_QM_app.Services;
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
        public FeedbackViewModel()
        {
            Title = "Feedback (1 - 9)";
            SendFeedbackCommand = new Command(RateFeedbackAsync);
            ResetFeedbackCommand = new Command(ResetFeedback);
            rating = new Rating();
            rating.RatingId = Guid.NewGuid().ToString();

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
                var order = await App.OrdersDataStore.GetItemAsync(orderId);
                Order = order;

            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        private async void RateFeedbackAsync()
        {
            // TODO abstract dialog_service
            bool answer = await Shell.Current.DisplayAlert("Confirmation", "Send feedback?", "Yes", "No");

            // TODO parameter
            if (answer)
            {
                // update                
                Order.Status = Status.rated;
                Order.Rating = Rating;
                await App.OrdersDataStore.UpdateItemAsync(Order);

                // send mqtt
                JsonSerializerOptions options = new JsonSerializerOptions()
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
                };

                string res = JsonSerializer.Serialize<Order>(Order, options);

                await MqttConnectionService.HandlePublishMessage("prod/orders/rated", res);

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
