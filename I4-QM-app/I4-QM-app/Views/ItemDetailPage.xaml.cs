using I4_QM_app.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace I4_QM_app.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}