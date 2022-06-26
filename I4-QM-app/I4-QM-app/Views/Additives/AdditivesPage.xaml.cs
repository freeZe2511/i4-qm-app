using I4_QM_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace I4_QM_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdditivesPage : ContentPage
    {
        AdditivesViewModel _viewModel;
        public AdditivesPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new AdditivesViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}