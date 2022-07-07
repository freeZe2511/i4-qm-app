using System;
using Xamarin.Forms;

namespace I4_QM_app.ViewModels
{
    /// <summary>
    /// ViewModel for Settings Page.
    /// </summary>
    // TODO
    public class SettingsViewModel : BaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// </summary>
        public SettingsViewModel()
        {
            Title = "Settings";
            ToggleConnectionCommand = new Command(ToggleConnection);
            TogglePushNotificationsCommand = new Command(TogglePushNotifications);
            ChangeBrokerURLCommand = new Command(ChangeBrokerURL);
            ResetDatabaseCommand = new Command(ResetDatabase);
            ManualSyncCommand = new Command(ManualSync);
        }

        /// <summary>
        /// Gets command to toggle mqtt broker connection.
        /// </summary>
        public Command ToggleConnectionCommand { get; }

        /// <summary>
        /// Gets command to toggle push notifications.
        /// </summary>
        public Command TogglePushNotificationsCommand { get; }

        /// <summary>
        /// Gets command to change mqtt broker url.
        /// </summary>
        public Command ChangeBrokerURLCommand { get; }

        /// <summary>
        /// Gets command to delete/reset database.
        /// </summary>
        public Command ResetDatabaseCommand { get; }

        /// <summary>
        /// Gets command to manually sync app.
        /// </summary>
        public Command ManualSyncCommand { get; }

        /// <summary>
        /// Handles connection toggle.
        /// </summary>
        private void ToggleConnection(object sender)
        {
            // TODO
            Console.WriteLine(sender);
        }

        /// <summary>
        /// Handles push notifications toggle.
        /// </summary>
        private void TogglePushNotifications(object sender)
        {
            // TODO
            Console.WriteLine(sender);
        }

        /// <summary>
        /// Handles broker url change.
        /// </summary>
        private void ChangeBrokerURL(object sender)
        {
            // TODO
            Console.WriteLine(sender);
        }

        /// <summary>
        /// Handles reset database.
        /// </summary>
        private void ResetDatabase(object sender)
        {
            // TODO
            Console.WriteLine(sender);
        }

        /// <summary>
        /// Handles manual sync.
        /// </summary>
        private void ManualSync(object sender)
        {
            // TODO
            Console.WriteLine(sender);
        }
    }
}
