using I4_QM_app.Services.Abstract;
using I4_QM_app.Services.Connection;
using I4_QM_app.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    /// <summary>
    /// ViewModel for Login Page.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        private readonly IConnectionService connectionService;
        private readonly IAbstractService abstractService;

        private string entryValue;

        /// <summary>
        /// User id max length.
        /// </summary>
        public int IdLength = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginViewModel"/> class.
        /// </summary>
        /// <param name="connectionService">Connection Service.</param>
        /// <param name="abstractService">Abstract Service.</param>
        public LoginViewModel(IConnectionService connectionService, IAbstractService abstractService)
        {
            this.connectionService = connectionService;
            this.abstractService = abstractService;

            LoginCommand = new Command(OnLoginClicked, Validate);
            string userId = this.abstractService.GetPreferences("UserID", string.Empty);

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

            this.abstractService.SetPreferences("UserID", EntryValue);
            await connectionService.HandlePublishMessage("connected", EntryValue);
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            EntryValue = string.Empty;
        }
    }
}
