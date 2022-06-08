using I4_QM_app.Views;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }



        private async void OnLoginClicked(object obj)
        {

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
