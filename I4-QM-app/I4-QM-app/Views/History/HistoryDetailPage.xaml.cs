using I4_QM_app.ViewModels;
using Xamarin.Forms;

namespace I4_QM_app.Views
{
    /// <summary>
    /// Page for History Details.
    /// </summary>
    public partial class HistoryDetailPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryDetailPage"/> class.
        /// </summary>
        public HistoryDetailPage()
        {
            InitializeComponent();
            BindingContext = new HistoryDetailViewModel(App.OrdersDataService, App.NotificationService);
        }
    }
}