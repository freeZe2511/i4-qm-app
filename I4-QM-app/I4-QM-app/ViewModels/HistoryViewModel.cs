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
        public Command SortByID { get; }
        public Command SortByDone { get; }
        public Command SortByQty { get; }
        public Command SortByCreated { get; }
        public Command DisableCommand { get; }

        public bool Descending { get; set; }
        public HistoryViewModel()
        {
            Title = "History";
            Descending = true;
            History = new SortableObservableCollection<Order>() { SortingSelector = i => i.Status, Descending = Descending };

            // TODO maybe overloading main thread
            LoadHistoryCommand = new Command(async () => await ExecuteLoadHistoryCommand());

            OrderTapped = new Command<Order>(OnOrderSelected);
            DeleteAllItemsCommand = new Command(DeleteAllHistoryItems);

            SortByID = new Command(async () => await SortBy(i => i.Id));
            SortByDone = new Command(async () => await SortBy(i => i.Due));
            SortByQty = new Command(async () => await SortBy(i => i.Amount));
            SortByCreated = new Command(async () => await SortBy(i => i.Created));

            DisableCommand = new Command(execute: () => { }, canExecute: () => { return false; });

            // TODO maybe? not working like this
            //SortBy = new Command<Func<Order, object>>(
            //    execute: async (Func<Order, object> arg) =>
            //    {
            //        Console.WriteLine(arg);
            //        History.SortingSelector = arg;
            //        History.Descending = Descending;
            //        Descending = !Descending;
            //        await ExecuteLoadHistoryCommand();

            //    });
        }


        private async Task SortBy(Func<Order, object> predicate)
        {
            History.SortingSelector = predicate;
            History.Descending = Descending;
            Descending = !Descending;
            await ExecuteLoadHistoryCommand();
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

        async void DeleteAllHistoryItems()
        {
            // TODO abstract dialog_service
            bool answer = await Shell.Current.DisplayAlert("Confirmation", "Delete whole history?", "Yes", "No");

            // TODO parameter
            if (answer) await App.OrdersDataStore.DeleteManyItemsAsync();
            await ExecuteLoadHistoryCommand();

        }
    }

}
