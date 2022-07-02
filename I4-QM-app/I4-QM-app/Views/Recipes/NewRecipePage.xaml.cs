using I4_QM_app.ViewModels;
using Xamarin.Forms;

namespace I4_QM_app.Views
{
    public partial class NewRecipePage : ContentPage
    {
        NewRecipeViewModel _viewModel;

        public NewRecipePage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new NewRecipeViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}