using I4_QM_app.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        private Order _selectedOrder;
        public ObservableCollection<Order> History { get; }
        public Command LoadHistoryCommand { get; }
        public Command<Order> HistoryTapped { get; }
        public HistoryViewModel()
        {
            Title = "History";
            History = new ObservableCollection<Order>();
            // TODO maybe overloading main thread
            LoadHistoryCommand = new Command(async () => await ExecuteLoadHistoryCommand());

            HistoryTapped = new Command<Order>(OnOrderSelected);
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

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedOrder = null;
        }

        async Task ExecuteLoadHistoryCommand()
        {
            IsBusy = true;

            try
            {
                History.Clear();
                //var orders = await App.OrdersDataStore.GetItemsAsync(true);
                var history = await App.OrdersDataStore.GetItemsFilteredAsync(a => a.Status != Status.open);

                foreach (var order in history)
                {
                    //test filter
                    //if (order.Status == Status.done)
                    History.Add(order);
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

        async void OnOrderSelected(Order item)
        {
            if (item == null)
                return;

            // TODO abstract dialog_service
            //bool answer = await Shell.Current.DisplayAlert("Confirmation", "Start mixing now?", "Yes", "No");

            // This will push the ItemDetailPage onto the navigation stack
            //if (answer) await Shell.Current.GoToAsync($"{nameof(HistoryPage)}?{nameof(HistoryDetailViewModel.OrderId)}={item.Id}");

        }
    }
}
