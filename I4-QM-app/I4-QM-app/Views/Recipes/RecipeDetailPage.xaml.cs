using I4_QM_app.ViewModels;
using Xamarin.Forms;

namespace I4_QM_app.Views
{
    /// <summary>
    /// Page for Recipe Details.
    /// </summary>
    public partial class RecipeDetailPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeDetailPage"/> class.
        /// </summary>
        public RecipeDetailPage()
        {
            InitializeComponent();
            BindingContext = new RecipeDetailViewModel(App.RecipesDataService, App.NotificationService, App.AdditivesDataService);
        }
    }
}