using I4_QM_app.ViewModels.History;
using Xamarin.Forms;

namespace I4_QM_app.Views
{
    /// <summary>
    /// Page for History List.
    /// </summary>
    public partial class HistoryPage : ContentPage
    {
        private readonly HistoryViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryPage"/> class.
        /// </summary>
        public HistoryPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new HistoryViewModel(App.OrdersDataService, App.NotificationService, App.ConnectionService);
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