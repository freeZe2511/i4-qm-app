using I4_QM_app.Helpers;
using I4_QM_app.Models;
using I4_QM_app.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        private Order _selectedOrder;
        public SortableObservableCollection<Order> History { get; }
        public Command LoadHistoryCommand { get; }
        public Command DeleteAllItemsCommand { get; }
        public Command<Order> OrderTapped { get; }
        public HistoryViewModel()
        {
            Title = "History";
            History = new SortableObservableCollection<Order>() { SortingSelector = i => i.Status, Descending = true };

            // TODO maybe overloading main thread
            LoadHistoryCommand = new Command(async () => await ExecuteLoadHistoryCommand());

            OrderTapped = new Command<Order>(OnOrderSelected);
            DeleteAllItemsCommand = new Command(DeleteAllItems);
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
                var history = await App.OrdersDataStore.GetItemsFilteredAsync(a => a.Status != Status.open);

                foreach (var order in history)
                {
                    History.Add(order);
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
            await Shell.Current.GoToAsync($"{nameof(HistoryDetailPage)}?{nameof(HistoryDetailViewModel.OrderId)}={item.Id}");

        }

        async void DeleteAllItems()
        {
            // TODO abstract dialog_service
            bool answer = await Shell.Current.DisplayAlert("Confirmation", "Delete history?", "Yes", "No");

            if (answer) await App.OrdersDataStore.DeleteManyItemsAsync();
            await ExecuteLoadHistoryCommand();

        }
    }

}
