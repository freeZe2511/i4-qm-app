using I4_QM_app.Models;
using I4_QM_app.Views;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        private Order _selectedOrder;
        public SortableObservableCollection<Order> History { get; }
        public Command LoadHistoryCommand { get; }
        public Command<Order> OrderTapped { get; }
        public HistoryViewModel()
        {
            Title = "History";
            History = new SortableObservableCollection<Order>() { SortingSelector = i => i.Status, Descending = true };
            // TODO maybe overloading main thread
            LoadHistoryCommand = new Command(async () => await ExecuteLoadHistoryCommand());

            OrderTapped = new Command<Order>(OnOrderSelected);
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
    }

    // https://stackoverflow.com/questions/19112922/sort-observablecollectionstring-through-c-sharp
    public class SortableObservableCollection<T> : ObservableCollection<T>
    {
        public Func<T, object> SortingSelector { get; set; }
        public bool Descending { get; set; }
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (SortingSelector == null
                || e.Action == NotifyCollectionChangedAction.Remove
                || e.Action == NotifyCollectionChangedAction.Reset)
                return;

            var query = this
              .Select((item, index) => (Item: item, Index: index));
            query = Descending
              ? query.OrderBy(tuple => SortingSelector(tuple.Item))
              : query.OrderByDescending(tuple => SortingSelector(tuple.Item));

            var map = query.Select((tuple, index) => (OldIndex: tuple.Index, NewIndex: index))
             .Where(o => o.OldIndex != o.NewIndex);

            using (var enumerator = map.GetEnumerator())
                if (enumerator.MoveNext())
                    Move(enumerator.Current.OldIndex, enumerator.Current.NewIndex);


        }
    }
}
