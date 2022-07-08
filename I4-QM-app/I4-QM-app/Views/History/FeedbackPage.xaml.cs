using I4_QM_app.ViewModels;

using Xamarin.Forms;

namespace I4_QM_app.Views
{
    /// <summary>
    /// Page for Feedback rating.
    /// </summary>
    public partial class FeedbackPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedbackPage"/> class.
        /// </summary>
        public FeedbackPage()
        {
            InitializeComponent();
            BindingContext = new FeedbackViewModel(App.OrdersDataService, App.NotificationService, App.ConnectionService);
        }
    }
}