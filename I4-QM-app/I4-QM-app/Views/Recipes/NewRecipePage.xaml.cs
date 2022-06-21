using I4_QM_app.ViewModels;
using Xamarin.Forms;

namespace I4_QM_app.Views
{
    public partial class NewRecipePage : ContentPage
    {
        public NewRecipePage()
        {
            InitializeComponent();
            BindingContext = new NewRecipeViewModel();
        }
    }
}