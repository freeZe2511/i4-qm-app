using I4_QM_app.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private string userId;
        private int openOrdersCount;
        private int mixedOrdersCount;
        private int ratedOrdersCount;

        private DateTime nextOrder;
        private DateTime latestOrder;
        private DateTime oldestOrder;

        private int recipesCount;
        private int additivesCount;

        public HomeViewModel()
        {
            Title = "Home";

            RefreshCommand = new Command(async () => await LoadRefreshCommand());

        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        public Command RefreshCommand { get; }

        public string UserId
        {
            get => userId;
            set => SetProperty(ref userId, value);
        }

        public int OpenOrdersCount
        {
            get => openOrdersCount;
            set => SetProperty(ref openOrdersCount, value);
        }

        public DateTime NextOrder
        {
            get => nextOrder;
            set => SetProperty(ref nextOrder, value);
        }

        public DateTime LatestOrder
        {
            get => latestOrder;
            set => SetProperty(ref latestOrder, value);
        }

        public DateTime OldestOrder
        {
            get => oldestOrder;
            set => SetProperty(ref oldestOrder, value);
        }

        public int MixedOrdersCount
        {
            get => mixedOrdersCount;
            set => SetProperty(ref mixedOrdersCount, value);
        }

        public int RatedOrdersCount
        {
            get => ratedOrdersCount;
            set => SetProperty(ref ratedOrdersCount, value);
        }

        public int RecipesCount
        {
            get => recipesCount;
            set => SetProperty(ref recipesCount, value);
        }

        public int AdditivesCount
        {
            get => additivesCount;
            set => SetProperty(ref additivesCount, value);
        }

        private async Task LoadRefreshCommand()
        {
            IsBusy = true;

            try
            {
                var orders = await App.OrdersDataService.GetItemsAsync();
                var additives = await App.AdditivesDataService.GetItemsAsync();
                var recipes = await App.RecipesDataService.GetItemsAsync();

                UserId = Preferences.Get("UserID", string.Empty);

                OpenOrdersCount = orders.Count(x => x.Status == Status.Open);
                MixedOrdersCount = orders.Count(x => x.Status == Status.Mixed);
                RatedOrdersCount = orders.Count(x => x.Status == Status.Rated);

                var openOrders = orders.Where(x => x.Status == Status.Open);
                NextOrder = openOrders.Any() ? openOrders.Min(x => x.Due) : DateTime.MinValue;
                LatestOrder = openOrders.Any() ? openOrders.Max(x => x.Received) : DateTime.MinValue;
                OldestOrder = openOrders.Any() ? openOrders.Min(x => x.Received) : DateTime.MinValue;

                RecipesCount = recipes.Count();
                AdditivesCount = additives.Count();
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