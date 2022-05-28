using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class OrderDetailViewModel : BaseViewModel
    {
        private string itemId;
        private string text;
        private string description;
        public string Id { get; set; }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadOrderId(value);
            }
        }

        public async void LoadOrderId(string itemId)
        {
            try
            {
                var item = await DataStore.GetOrderAsync(itemId);
                Id = item.Id;
                Text = item.Id;
                Description = item.UserId;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

    }
}
