using I4_QM_app.ViewModels;
using Xamarin.Forms;

namespace I4_QM_app.Views
{
    /// <summary>
    /// Page for Recipes List.
    /// </summary>
    public partial class RecipesPage : ContentPage
    {
        private readonly RecipesViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipesPage"/> class.
        /// </summary>
        public RecipesPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new RecipesViewModel(App.RecipesDataService, App.NotificationService);
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