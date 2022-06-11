using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    [QueryProperty(nameof(OrderId), nameof(OrderId))]
    public class FeedbackViewModel
    {
        private string orderId;
        public FeedbackViewModel()
        {
            //FeedbackCommand = new Command(OnFeedbackClicked);
            //DeleteItemCommand = new Command(DeleteItem);
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

        public async void LoadOrderId(string orderId)
        {
            try
            {
                var order = await App.OrdersDataStore.GetItemAsync(orderId);


            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
