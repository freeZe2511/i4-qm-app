using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public Command ToggleConnectionCommand { get; }
        public SettingsViewModel()
        {
            Title = "Settings";
            ToggleConnectionCommand = new Command(ToggleConnection);
        }

        //public bool IsConnected { get => App.ConnectionService.IsConnected; }

        public void ToggleConnection()
        {
            //App.ConnectionService.ToggleMqttClient();
        }

    }
}
