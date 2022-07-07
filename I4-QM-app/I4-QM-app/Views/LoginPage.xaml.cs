using I4_QM_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace I4_QM_app.Views
{
    /// <summary>
    /// Page for Login.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginPage"/> class.
        /// </summary>
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
        }
    }
}