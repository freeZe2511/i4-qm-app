
using I4_QM_app.ViewModels;
using Xamarin.Forms;

namespace I4_QM_app.Views
{

    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = new SettingsViewModel();
        }
    }
}