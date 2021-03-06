using I4_QM_app.Models;
using I4_QM_app.Services.Abstract;
using I4_QM_app.Services.Data;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    /// <summary>
    /// ViewModel for HomePage.
    /// </summary>
    public class HomeViewModel : BaseViewModel
    {
        private readonly IDataService<Order> ordersService;
        private readonly IDataService<Recipe> recipesService;
        private readonly IDataService<Additive> additivesService;
        private readonly IAbstractService abstractService;

        private string userId;
        private int openOrdersCount;
        private int mixedOrdersCount;
        private int ratedOrdersCount;

        private DateTime nextOrder;
        private DateTime latestOrder;
        private DateTime oldestOrder;

        private int recipesCount;
        private int additivesCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        /// <param name="ordersService">Orders Service.</param>
        /// <param name="recipesService">Recipes Service.</param>
        /// <param name="additivesService">Additives Service.</param>
        public HomeViewModel(IDataService<Order> ordersService, IDataService<Recipe> recipesService, IDataService<Additive> additivesService, IAbstractService abstractService)
        {
            this.ordersService = ordersService;
            this.recipesService = recipesService;
            this.additivesService = additivesService;
            this.abstractService = abstractService;

            Title = "Home";
            RefreshCommand = new Command(async () => await LoadRefreshCommand());
        }

        /// <summary>
        /// Gets command to refresh data from db.
        /// </summary>
        public Command RefreshCommand { get; }

        /// <summary>
        /// Gets or sets user id.
        /// </summary>
        public string UserId
        {
            get => userId;
            set => SetProperty(ref userId, value);
        }

        /// <summary>
        /// Gets or sets open orders count.
        /// </summary>
        public int OpenOrdersCount
        {
            get => openOrdersCount;
            set => SetProperty(ref openOrdersCount, value);
        }

        /// <summary>
        /// Gets or sets mixed orders count.
        /// </summary>
        public int MixedOrdersCount
        {
            get => mixedOrdersCount;
            set => SetProperty(ref mixedOrdersCount, value);
        }

        /// <summary>
        /// Gets or sets rated orders count.
        /// </summary>
        public int RatedOrdersCount
        {
            get => ratedOrdersCount;
            set => SetProperty(ref ratedOrdersCount, value);
        }

        /// <summary>
        /// Gets or sets next order date.
        /// </summary>
        public DateTime NextOrder
        {
            get => nextOrder;
            set => SetProperty(ref nextOrder, value);
        }

        /// <summary>
        /// Gets or sets latest order date.
        /// </summary>
        public DateTime LatestOrder
        {
            get => latestOrder;
            set => SetProperty(ref latestOrder, value);
        }

        /// <summary>
        /// Gets or sets oldest order date.
        /// </summary>
        public DateTime OldestOrder
        {
            get => oldestOrder;
            set => SetProperty(ref oldestOrder, value);
        }

        /// <summary>
        /// Gets or sets recipes count.
        /// </summary>
        public int RecipesCount
        {
            get => recipesCount;
            set => SetProperty(ref recipesCount, value);
        }

        /// <summary>
        /// Gets or sets additives count.
        /// </summary>
        public int AdditivesCount
        {
            get => additivesCount;
            set => SetProperty(ref additivesCount, value);
        }

        /// <summary>
        /// Sets isBusy onAppearing.
        /// </summary>
        public void OnAppearing()
        {
            IsBusy = true;
        }

        /// <summary>
        /// Loads data from db.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task LoadRefreshCommand()
        {
            IsBusy = true;

            try
            {
                var orders = await ordersService.GetItemsAsync();
                var additives = await additivesService.GetItemsAsync();
                var recipes = await recipesService.GetItemsAsync();

                UserId = abstractService.GetPreferences("UserID", string.Empty);

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