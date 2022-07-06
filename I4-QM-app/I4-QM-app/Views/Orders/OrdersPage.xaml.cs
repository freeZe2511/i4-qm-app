using I4_QM_app.ViewModels;
using Xamarin.Forms;

namespace I4_QM_app.Views
{
    /// <summary>
    /// Page for Orders List.
    /// </summary>
    public partial class OrdersPage : ContentPage
    {
        private readonly OrdersViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersPage"/> class.
        /// </summary>
        public OrdersPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new OrdersViewModel();
        }

        /// <summary>
        /// Sets base and viewmodel.
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.OnAppearing();
        }
    }
}