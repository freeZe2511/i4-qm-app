using I4_QM_app.ViewModels.Recipes;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace I4_QM_app.Views.Recipes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransformRecipePage : ContentPage
    {
        public TransformRecipePage()
        {
            InitializeComponent();
            BindingContext = new TransformRecipeViewModel();
        }

    }
}