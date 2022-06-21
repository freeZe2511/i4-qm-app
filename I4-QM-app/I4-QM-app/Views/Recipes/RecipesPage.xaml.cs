using I4_QM_app.ViewModels;
using Xamarin.Forms;

namespace I4_QM_app.Views
{

    public partial class RecipesPage : ContentPage
    {
        RecipesViewModel _viewModel;
        public RecipesPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new RecipesViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}