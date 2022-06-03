using I4_QM_app.Models;
using I4_QM_app.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        private Order _selectedOrder;

        public ObservableCollection<Order> Orders { get; }
        public Command LoadOrdersCommand { get; }
        public Command<Order> OrderTapped { get; }

        public string OrdersCount { get => Orders.Count.ToString(); }

        public OrdersViewModel()
        {
            // maybe bad binding atm
            Title = "Orders";
            Orders = new ObservableCollection<Order>();
            // TODO maybe overloading main thread
            LoadOrdersCommand = new Command(async () => await ExecuteLoadOrdersCommand());

            OrderTapped = new Command<Order>(OnOrderSelected);
        }

        async Task ExecuteLoadOrdersCommand()
        {
            IsBusy = true;

            try
            {
                Orders.Clear();
                var orders = await App.OrdersDataStore.GetItemsFilteredAsync(a => a.Status == Status.open);

                foreach (var order in orders)
                {
                    //test filter
                    //if (order.Status == Status.open)
                    Orders.Add(order);
                    // TODO sort
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
            SelectedOrder = null;
        }

        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                SetProperty(ref _selectedOrder, value);
                OnOrderSelected(value);
            }
        }

        async void OnOrderSelected(Order item)
        {
            if (item == null)
                return;

            // TODO abstract dialog_service
            bool answer = await Shell.Current.DisplayAlert("Confirmation", "Start mixing now?", "Yes", "No");

            // This will push the ItemDetailPage onto the navigation stack
            if (answer) await Shell.Current.GoToAsync($"{nameof(OrderDetailPage)}?{nameof(OrderDetailViewModel.OrderId)}={item.Id}");

        }

    }
}