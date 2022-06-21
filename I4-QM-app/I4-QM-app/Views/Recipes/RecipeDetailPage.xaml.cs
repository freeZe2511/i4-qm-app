using I4_QM_app.ViewModels;

using Xamarin.Forms;

namespace I4_QM_app.Views
{
    public partial class RecipeDetailPage : ContentPage
    {
        public RecipeDetailPage()
        {
            InitializeComponent();
            BindingContext = new RecipeDetailViewModel();
        }
    }
}