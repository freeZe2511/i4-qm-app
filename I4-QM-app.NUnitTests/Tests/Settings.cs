using I4_QM_app.ViewModels;
using NUnit.Framework;

namespace I4_QM_app.NUnitTests.Tests
{
    internal class Settings
    {
        [Test]
        public void SettingsInit()
        {
            var settings = new SettingsViewModel();
            Assert.IsNotNull(settings);
            Assert.IsNotNull(settings.ToggleConnectionCommand);
            Assert.IsNotNull(settings.TogglePushNotificationsCommand);
            Assert.IsNotNull(settings.ChangeBrokerURLCommand);
            Assert.IsNotNull(settings.ResetDatabaseCommand);
            Assert.IsNotNull(settings.ManualSyncCommand);
        }

        [Test]
        public void SettingsToggleConnection()
        {
            var settings = new SettingsViewModel();
            Assert.IsTrue(settings.ToggleConnectionCommand.CanExecute(null));
            settings.ToggleConnectionCommand.Execute(null);
        }

        [Test]
        public void SettingsTogglePushNotifications()
        {
            var settings = new SettingsViewModel();
            Assert.IsTrue(settings.TogglePushNotificationsCommand.CanExecute(null));
            settings.TogglePushNotificationsCommand.Execute(null);
        }

        [Test]
        public void SettingsChangeBrokerURL()
        {
            var settings = new SettingsViewModel();
            Assert.IsTrue(settings.ChangeBrokerURLCommand.CanExecute(null));
            settings.ChangeBrokerURLCommand.Execute(null);
        }

        [Test]
        public void SettingsResetDatabase()
        {
            var settings = new SettingsViewModel();
            Assert.IsTrue(settings.ResetDatabaseCommand.CanExecute(null));
            settings.ResetDatabaseCommand.Execute(null);
        }

        [Test]
        public void SettingsManualSync()
        {
            var settings = new SettingsViewModel();
            Assert.IsTrue(settings.ManualSyncCommand.CanExecute(null));
            settings.ManualSyncCommand.Execute(null);
        }
    }
}
