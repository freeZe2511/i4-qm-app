using I4_QM_app.Views;
using Xamarin.Forms;
// using System.Windows.Forms;

namespace I4_QM_app.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public string EntryValue { get; set; }
        public int UID { get; set; }
        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }



        private async void OnLoginClicked(object obj)
        {
            //Application.Current.Properties["UserID"]

            if(!int.TryParse(EntryValue, out int UID))
            {
                EntryValue = "";
               //MessageBox.Show("Only decimal numbers allowed. Please, try agian.", "Invalid UserID", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
           
            }

            if (UID <= 0)
            {
                EntryValue = "";
                return;
            }

            ((App)App.Current).CurrentUser = new Models.User(EntryValue);
            EntryValue = "";

            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");


        }
    }

    // TODO
    // https://stackoverflow.com/questions/50260271/xamarin-detecting-enter-key-mvvm

    //public class EventToCommandBehavior : Behavior<Entry>
    //{
    //    public static readonly BindableProperty EventNameProperty =
    //      BindableProperty.Create("EventName", typeof(string), typeof(EventToCommandBehavior), null, propertyChanged: OnEventNameChanged);
    //    public static readonly BindableProperty CommandProperty =
    //      BindableProperty.Create("Command", typeof(ICommand), typeof(EventToCommandBehavior), null);
    //    public static readonly BindableProperty CommandParameterProperty =
    //      BindableProperty.Create("CommandParameter", typeof(object), typeof(EventToCommandBehavior), null);
    //    public static readonly BindableProperty InputConverterProperty =
    //      BindableProperty.Create("Converter", typeof(IValueConverter), typeof(EventToCommandBehavior), null);

    //    public string EventName { }
    //    public ICommand Command { }
    //    public object CommandParameter { }
    //    public IValueConverter Converter { }

    //}

}
