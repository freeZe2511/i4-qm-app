using I4_QM_app.ViewModels.Recipes;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace I4_QM_app.Views.Recipes
{
    /// <summary>
    /// Page for recipe to order transformation.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransformRecipePage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformRecipePage"/> class.
        /// </summary>
        public TransformRecipePage()
        {
            InitializeComponent();
            BindingContext = new TransformRecipeViewModel(App.OrdersDataService, App.NotificationService, App.RecipesDataService);
        }
    }
}