using I4_QM_app.ViewModels;

using Xamarin.Forms;

namespace I4_QM_app.Views
{
    public partial class HistoryDetailPage : ContentPage
    {
        public HistoryDetailPage()
        {
            InitializeComponent();
            BindingContext = new HistoryDetailViewModel();
        }
    }
}