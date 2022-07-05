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

        public Command<Order> OrderTapped { get; }

        public Command DisableCommand { get; }

        public Command SortByCommand { get; }

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

            SortByCommand = new Command<string>(
                execute: async (string arg) =>
                {
                    arg = arg.Trim();

                    // works
                    if (arg == "Id") await SortBy(i => i.Id);
                    if (arg == "Due") await SortBy(i => i.Due);
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

            DisableCommand = new Command(execute: () => { }, canExecute: () => { return false; });


        }

        private async Task SortBy(Func<Order, object> predicate)
        {
            Orders.SortingSelector = predicate;
            Orders.Descending = Descending;
            Descending = !Descending;
            await ExecuteLoadOrdersCommand();
        }

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

            bool answer = await App.NotificationService.ShowSimpleDisplayAlert("Confirmation", "Start mixing now?", "Yes", "No");

            // This will push the ItemDetailPage onto the navigation stack
            if (answer) await Shell.Current.GoToAsync($"{nameof(OrderDetailPage)}?{nameof(OrderDetailViewModel.OrderId)}={item.Id}");

        }

    }
}