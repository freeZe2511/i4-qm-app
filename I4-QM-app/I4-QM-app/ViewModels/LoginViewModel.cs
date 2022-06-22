using I4_QM_app.Models;
using I4_QM_app.Views;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string entryValue;
        public int IdLength = 4;
        public int UID { get; set; }
        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked, Validate);

            string userId = Preferences.Get("UserID", "null");

            if (userId != "null")
            {
                Task.Run(async () =>
                {
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                    await App.ConnectionService.HandlePublishMessage("connected", userId);
                });
            }

            this.PropertyChanged +=
                (_, __) => LoginCommand.ChangeCanExecute();
        }


        public string EntryValue
        {
            get => entryValue;
            set => SetProperty(ref entryValue, value);
        }

        private bool Validate(object arg)
        {
            return !String.IsNullOrWhiteSpace(EntryValue)
                && int.TryParse(EntryValue, out int UID)
                && UID > 0
                && EntryValue.Length == IdLength;
        }


        private async void OnLoginClicked(object obj)
        {
            if (!int.TryParse(EntryValue, out int UID) || String.IsNullOrWhiteSpace(EntryValue) || UID <= 0 || EntryValue.Length != IdLength)
            {
                EntryValue = "";
                //MessageBox.Show("Only decimal numbers allowed. Please, try agian.", "Invalid UserID", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;

            }

            ((App)App.Current).CurrentUser = new User(EntryValue);
            Preferences.Set("UserID", EntryValue);

            await App.ConnectionService.HandlePublishMessage("connected", EntryValue);

            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            EntryValue = "";
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
