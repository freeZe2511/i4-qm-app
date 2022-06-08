using I4_QM_app.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace I4_QM_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryDetailPage : ContentPage
    {
        public HistoryDetailPage()
        {
            InitializeComponent();
            BindingContext = new HistoryDetailViewModel();
        }
    }
}