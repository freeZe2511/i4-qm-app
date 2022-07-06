using I4_QM_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace I4_QM_app.Views
{
    /// <summary>
    /// Page for Additives List.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdditivesPage : ContentPage
    {
        private readonly AdditivesViewModel viewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditivesPage"/> class.
        /// </summary>
        public AdditivesPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new AdditivesViewModel();
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