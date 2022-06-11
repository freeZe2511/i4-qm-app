using I4_QM_app.ViewModels;

using Xamarin.Forms;

namespace I4_QM_app.Views
{
    public partial class FeedbackPage : ContentPage
    {
        public FeedbackPage()
        {
            InitializeComponent();
            BindingContext = new FeedbackViewModel();
        }
    }
}