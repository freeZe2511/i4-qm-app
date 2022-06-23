using I4_QM_app.Helpers;
using I4_QM_app.Models;
using I4_QM_app.Views;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        public Command DisableCommand { get; }
        public Command SortByCommand { get; }

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
            DisableCommand = new Command(execute: () => { }, canExecute: () => { return false; });

            SortByCommand = new Command<string>(
                execute: async (string arg) =>
                {
                    arg = arg.Trim();

                    // works
                    if (arg == "Id") await SortBy(i => i.Id);
                    if (arg == "Status") await SortBy(i => i.Status);
                    if (arg == "Done") await SortBy(i => i.Done);
                    if (arg == "Amount") await SortBy(i => i.Amount);
                    if (arg == "Received") await SortBy(i => i.Received);


                    // https://stackoverflow.com/questions/16213005/how-to-convert-a-lambdaexpression-to-typed-expressionfunct-t
                    // only works with id?! Specified cast is not valid
                    //if (typeof(Order).GetProperty(arg) != null)
                    //{
                    //    ParameterExpression parameter = Expression.Parameter(typeof(Order), "i");
                    //    MemberExpression memberExpression = Expression.Property(parameter, typeof(Order).GetProperty(arg));
                    //    LambdaExpression lambda = Expression.Lambda(memberExpression, parameter);

                    //    Console.WriteLine(lambda.ToString());

                    //    await SortBy((Func<Order, object>)lambda.Compile());
                    //}

                    // https://stackoverflow.com/questions/10655761/convert-string-into-func
                    // compiler error
                    //var str = "i => i." + arg;
                    //Console.WriteLine(str);
                    //var func = await CSharpScript.EvaluateAsync<Func<Order, object>>(str);
                    //await SortBy(func);


                });
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
                var history = await App.OrdersDataService.GetItemsFilteredAsync(a => a.Status != Status.open);

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

            bool answer = await App.NotificationService.ShowSimpleDisplayAlert("Confirmation", "Start mixing now?", "Yes", "No");

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(HistoryDetailPage)}?{nameof(HistoryDetailViewModel.OrderId)}={item.Id}");

        }

        async void DeleteAllHistoryItems()
        {
            bool answer = await App.NotificationService.ShowSimpleDisplayAlert("Confirmation", "Delete whole history?", "Yes", "No");

            var orders = await App.OrdersDataService.GetItemsFilteredAsync(x => x.Status != Status.open);

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            string ordersString = System.Text.Json.JsonSerializer.Serialize(orders, options);

            await App.ConnectionService.HandlePublishMessage("backup/orders/history", ordersString);

            if (answer) await App.OrdersDataService.DeleteManyItemsAsync(x => x.Status != Status.open);
            await ExecuteLoadHistoryCommand();

        }
    }

}
