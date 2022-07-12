using I4_QM_app.ViewModels.Recipes;
using Xamarin.Forms;

namespace I4_QM_app.Views
{
    /// <summary>
    /// Page for new Recipe Form.
    /// </summary>
    public partial class NewRecipePage : ContentPage
    {
        private readonly NewRecipeViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewRecipePage"/> class.
        /// </summary>
        public NewRecipePage()
        {
            InitializeComponent();
            BindingContext = viewModel = new NewRecipeViewModel(App.NotificationService, App.ConnectionService, App.AdditivesDataService, App.RecipesDataService, App.AbstractService);
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