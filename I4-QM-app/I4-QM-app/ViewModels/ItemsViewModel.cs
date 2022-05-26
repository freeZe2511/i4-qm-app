using I4_QM_app.Models;
using I4_QM_app.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private Order _selectedItem;

        public ObservableCollection<Order> Items { get; }
        public Command LoadItemsCommand { get; }
        //public Command AddItemCommand { get; }
        public Command<Order> ItemTapped { get; }

        public ItemsViewModel()
        {
            Title = "Orders";
            Items = new ObservableCollection<Order>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Order>(OnItemSelected);

            //AddItemCommand = new Command(OnAddItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public Order SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        //private async void OnAddItem(object obj)
        //{
        //    await Shell.Current.GoToAsync(nameof(NewItemPage));
        //}

        async void OnItemSelected(Order item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }
    }
}