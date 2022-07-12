using I4_QM_app.ViewModels;
using Xamarin.Forms;

namespace I4_QM_app.Views
{
    /// <summary>
    /// Page for HomePage.
    /// </summary>
    public partial class HomePage : ContentPage
    {
        private readonly HomeViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomePage"/> class.
        /// </summary>
        public HomePage()
        {
            InitializeComponent();
            BindingContext = viewModel = new HomeViewModel(App.OrdersDataService, App.RecipesDataService, App.AdditivesDataService, App.AbstractService);
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