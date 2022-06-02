using I4_QM_app.ViewModels;
using Xamarin.Forms;

namespace I4_QM_app.Views
{

    public partial class RecipesPage : ContentPage
    {
        RecipeViewModel _viewModel;
        public RecipesPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new RecipeViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}