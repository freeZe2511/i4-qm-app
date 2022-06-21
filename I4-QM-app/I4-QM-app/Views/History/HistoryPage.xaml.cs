
using I4_QM_app.ViewModels;
using Xamarin.Forms;

namespace I4_QM_app.Views
{

    public partial class HistoryPage : ContentPage
    {
        HistoryViewModel _viewModel;
        public HistoryPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new HistoryViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}