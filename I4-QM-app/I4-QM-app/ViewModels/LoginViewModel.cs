using I4_QM_app.Models;
using I4_QM_app.Services;
using I4_QM_app.Views;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    /// <summary>
    /// ViewModel for Login Page.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        private readonly IConnectionService connectionService;

        private string entryValue;

        /// <summary>
        /// User id max length.
        /// </summary>
        public int IdLength = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        /// <param name="connectionService">Connection Service.</param>
        public LoginViewModel(IConnectionService connectionService)
        {
            this.connectionService = connectionService;

            LoginCommand = new Command(OnLoginClicked, Validate);
            string userId = Preferences.Get("UserID", string.Empty);

            if (userId != string.Empty)
            {
                Task.Run(async () =>
                {
                    await connectionService.HandlePublishMessage("connected", userId);
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                });
            }

            this.PropertyChanged +=
                (_, __) => LoginCommand.ChangeCanExecute();
        }

        /// <summary>
        /// Gets or sets the User id.
        /// </summary>
        public int UID { get; set; }

        /// <summary>
        /// Gets command to login.
        /// </summary>
        public Command LoginCommand { get; }

        /// <summary>
        /// Gets or sets the login entry value.
        /// </summary>
        public string EntryValue
        {
            get => entryValue;
            set => SetProperty(ref entryValue, value);
        }

        /// <summary>
        /// Validation of entry if user id is valid.
        /// </summary>
        /// <param name="arg">Arg.</param>
        /// <returns>bool.</returns>
        private bool Validate(object arg)
        {
            return !string.IsNullOrWhiteSpace(EntryValue)
                && int.TryParse(EntryValue, out int uid)
                && uid > 0
                && EntryValue.Length == IdLength;
        }

        /// <summary>
        /// Handler to login.
        /// </summary>
        /// <param name="obj">Object.</param>
        private async void OnLoginClicked(object obj)
        {
            if (!int.TryParse(EntryValue, out int uid) || string.IsNullOrWhiteSpace(EntryValue) || uid <= 0 || EntryValue.Length != IdLength)
            {
                EntryValue = string.Empty;
                return;
            }

            ((App)App.Current).CurrentUser = new User(EntryValue);
            Preferences.Set("UserID", EntryValue);

            await connectionService.HandlePublishMessage("connected", EntryValue);

            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            EntryValue = string.Empty;
        }
    }
}
