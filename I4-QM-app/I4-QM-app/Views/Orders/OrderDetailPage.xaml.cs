using I4_QM_app.ViewModels;
using Xamarin.Forms;

namespace I4_QM_app.Views
{
    /// <summary>
    /// Page for Order Details.
    /// </summary>
    public partial class OrderDetailPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDetailPage"/> class.
        /// </summary>
        public OrderDetailPage()
        {
            InitializeComponent();
            BindingContext = new OrderDetailViewModel(App.OrdersDataService, App.NotificationService, App.ConnectionService, App.AdditivesDataService);
        }

    }
}