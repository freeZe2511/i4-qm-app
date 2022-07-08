using I4_QM_app.Helpers;
using I4_QM_app.Models;
using I4_QM_app.Services;
using I4_QM_app.Views;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    /// <summary>
    /// ViewModel for History ListPage.
    /// </summary>
    public class HistoryViewModel : BaseViewModel
    {
        private readonly IDataService<Order> ordersService;
        private readonly INotificationService notificationService;
        private readonly IConnectionService connectionService;

        private Order selectedOrder;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryViewModel"/> class.
        /// </summary>
        /// <param name="ordersService">Orders Service.</param>
        /// <param name="notificationService">Notifications Service.</param>
        /// <param name="connectionService">Connection Service.</param>
        public HistoryViewModel(IDataService<Order> ordersService, INotificationService notificationService, IConnectionService connectionService)
        {
            this.ordersService = ordersService;
            this.notificationService = notificationService;
            this.connectionService = connectionService;

            Title = "History";
            Descending = true;
            History = new SortableObservableCollection<Order>() { SortingSelector = i => i.Status, Descending = Descending };

            LoadHistoryCommand = new Command(async () => await ExecuteLoadHistoryCommand());
            OrderTapped = new Command<Order>(OnOrderSelected);
            DeleteAllItemsCommand = new Command(async () => await DeleteAllHistoryItems());
            DisableCommand = new Command(execute: () => { }, canExecute: () => { return false; });

            SortByCommand = new Command<string>(
                execute: async (string arg) =>
                {
                    arg = arg.Trim();

                    // works for now
                    if (arg == "Id")
                    {
                        await SortBy(i => i.Id);
                    }

                    if (arg == "Status")
                    {
                        await SortBy(i => i.Status);
                    }

                    if (arg == "Done")
                    {
                        await SortBy(i => i.Done);
                    }

                    if (arg == "Amount")
                    {
                        await SortBy(i => i.Amount);
                    }

                    if (arg == "Received")
                    {
                        await SortBy(i => i.Received);
                    }

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

        /// <summary>
        /// Gets history collection.
        /// </summary>
        public SortableObservableCollection<Order> History { get; }

        /// <summary>
        /// Gets command to load orders from db.
        /// </summary>
        public Command LoadHistoryCommand { get; }

        /// <summary>
        /// Gets command to delete all items from history.
        /// </summary>
        public Command DeleteAllItemsCommand { get; }

        /// <summary>
        /// Gets command to determine the tapped order.
        /// </summary>
        public Command<Order> OrderTapped { get; }

        /// <summary>
        /// Gets command to disable.
        /// </summary>
        public Command DisableCommand { get; }

        /// <summary>
        /// Gets command to sort collection.
        /// </summary>
        public Command SortByCommand { get; }

        /// <summary>
        /// Gets or sets a value indicating whether collection is sorted descending or ascending.
        /// </summary>
        public bool Descending { get; set; }

        /// <summary>
        /// Gets or sets the selected order.
        /// </summary>
        public Order SelectedOrder
        {
            get => selectedOrder;
            set
            {
                SetProperty(ref selectedOrder, value);
                OnOrderSelected(value);
            }
        }

        /// <summary>
        /// Sets IsBusy and selectedOrder when onAppearing.
        /// </summary>
        public void OnAppearing()
        {
            IsBusy = true;
            SelectedOrder = null;
        }

        /// <summary>
        /// Load orders (history) from db.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task ExecuteLoadHistoryCommand()
        {
            IsBusy = true;

            try
            {
                History.Clear();
                var history = await ordersService.GetItemsFilteredAsync(a => a.Status != Status.Open);

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

        /// <summary>
        /// Navigate to selected order history detail page.
        /// </summary>
        /// <param name="item">Order.</param>
        private async void OnOrderSelected(Order item)
        {
            if (item == null)
            {
                return;
            }

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(HistoryDetailPage)}?{nameof(HistoryDetailViewModel.OrderId)}={item.Id}");
        }

        /// <summary>
        /// Delete history and send mqtt backup.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task DeleteAllHistoryItems()
        {
            bool answer = await notificationService.ShowSimpleDisplayAlert("Confirmation", "Delete whole history?", "Yes", "No");

            var orders = await ordersService.GetItemsFilteredAsync(x => x.Status != Status.Open);

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            };

            string ordersString = JsonSerializer.Serialize(orders, options);

            await connectionService.HandlePublishMessage("backup/orders/history", ordersString);

            if (answer)
            {
                await ordersService.DeleteManyItemsAsync(x => x.Status != Status.Open);
            }

            await ExecuteLoadHistoryCommand();
        }

        /// <summary>
        /// Sort collection from predicate.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Task.</returns>
        private async Task SortBy(Func<Order, object> predicate)
        {
            History.SortingSelector = predicate;
            History.Descending = Descending;
            Descending = !Descending;
            await ExecuteLoadHistoryCommand();
        }
    }
}
