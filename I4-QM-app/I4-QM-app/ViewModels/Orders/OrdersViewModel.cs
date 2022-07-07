using I4_QM_app.Helpers;
using I4_QM_app.Models;
using I4_QM_app.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    /// <summary>
    /// ViewModel for Orders ListPage.
    /// </summary>
    public class OrdersViewModel : BaseViewModel
    {
        private Order selectedOrder;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersViewModel"/> class.
        /// </summary>
        public OrdersViewModel()
        {
            Title = "Orders";
            Descending = true;
            Orders = new SortableObservableCollection<Order>() { SortingSelector = i => i.Due, Descending = Descending };
            LoadOrdersCommand = new Command(async () => await ExecuteLoadOrdersCommand());

            OrderTapped = new Command<Order>(OnOrderSelected);

            SortByCommand = new Command<string>(
                execute: async (string arg) =>
                {
                    arg = arg.Trim();

                    // works for now
                    if (arg == "Id")
                    {
                        await SortBy(i => i.Id);
                    }

                    if (arg == "Due")
                    {
                        await SortBy(i => i.Due);
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

            DisableCommand = new Command(execute: () => { }, canExecute: () => { return false; });
        }

        /// <summary>
        /// Gets the orders collection.
        /// </summary>
        public SortableObservableCollection<Order> Orders { get; }

        /// <summary>
        /// Gets command to load orders from db.
        /// </summary>
        public Command LoadOrdersCommand { get; }

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
        /// Navigate to selected order detail page.
        /// </summary>
        /// <param name="item">Order.</param>
        private async void OnOrderSelected(Order item)
        {
            if (item == null)
            {
                return;
            }

            bool answer = await App.NotificationService.ShowSimpleDisplayAlert("Confirmation", "Start mixing now?", "Yes", "No");

            // This will push the ItemDetailPage onto the navigation stack
            if (answer)
            {
                await Shell.Current.GoToAsync($"{nameof(OrderDetailPage)}?{nameof(OrderDetailViewModel.OrderId)}={item.Id}");
            }
        }

        /// <summary>
        /// Sort collection from predicate.
        /// </summary>
        /// <param name="predicate">Predicate.</param>
        /// <returns>Task.</returns>
        private async Task SortBy(Func<Order, object> predicate)
        {
            Orders.SortingSelector = predicate;
            Orders.Descending = Descending;
            Descending = !Descending;
            await ExecuteLoadOrdersCommand();
        }

        /// <summary>
        /// Load orders from db.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task ExecuteLoadOrdersCommand()
        {
            IsBusy = true;

            try
            {
                Orders.Clear();
                var orders = await App.OrdersDataService.GetItemsFilteredAsync(a => a.Status == Status.Open);

                foreach (var order in orders)
                {
                    Orders.Add(order);
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
    }
}