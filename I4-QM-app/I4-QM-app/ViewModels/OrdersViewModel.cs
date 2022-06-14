using I4_QM_app.Helpers;
using I4_QM_app.Models;
using I4_QM_app.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        private Order _selectedOrder;

        public SortableObservableCollection<Order> Orders { get; }
        public Command LoadOrdersCommand { get; }
        public Command SortByID { get; }
        public Command SortByDue { get; }
        public Command SortByQty { get; }
        public Command SortByCreated { get; }
        public Command<Order> OrderTapped { get; }

        public bool Descending { get; set; }

        public OrdersViewModel()
        {
            // maybe bad binding atm
            Title = "Orders";
            Descending = true;
            Orders = new SortableObservableCollection<Order>() { SortingSelector = i => i.Due, Descending = Descending };
            // TODO maybe overloading main thread
            LoadOrdersCommand = new Command(async () => await ExecuteLoadOrdersCommand());

            OrderTapped = new Command<Order>(OnOrderSelected);

            SortByID = new Command(async () => await SortBy(i => i.Id));
            SortByDue = new Command(async () => await SortBy(i => i.Due));
            SortByQty = new Command(async () => await SortBy(i => i.Amount));
            SortByCreated = new Command(async () => await SortBy(i => i.Created));


        }

        private async Task SortBy(Func<Order, object> predicate)
        {
            Orders.SortingSelector = predicate;
            Orders.Descending = Descending;
            Descending = !Descending;
            await ExecuteLoadOrdersCommand();
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

        public void SortOrder(object sender)
        {
            Console.WriteLine("EEEEEEEEEEE");
            Console.WriteLine((ToolbarItem)sender);
        }

    }
}