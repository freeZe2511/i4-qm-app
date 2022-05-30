using I4_QM_app.Models;
using I4_QM_app.ViewModels;
using Xamarin.Forms;

namespace I4_QM_app.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Order Order { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}