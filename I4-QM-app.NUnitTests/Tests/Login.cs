using I4_QM_app.Models;
using I4_QM_app.NUnitTests.MockServices;
using I4_QM_app.Services;
using I4_QM_app.Services.Abstract;
using I4_QM_app.ViewModels;
using NUnit.Framework;

namespace I4_QM_app.NUnitTests.Tests
{
    internal class Login
    {
        private IConnectionService connectionService;
        private IAbstractService abstractService;

        [SetUp]
        public void Setup()
        {
            connectionService = new MockConnectionService();
            abstractService = new MockAbstractService();
        }

        [Test]
        public void LoginSuccess()
        {
            var login = new LoginViewModel(connectionService, abstractService);
            Assert.IsNull(login.EntryValue);
            Assert.IsEmpty(abstractService.GetPreferences("UserID", string.Empty));
            Assert.IsFalse(login.LoginCommand.CanExecute(null));
            login.EntryValue = "1234";
            Assert.IsTrue(login.LoginCommand.CanExecute(null));
            login.LoginCommand.Execute(null);
            Assert.AreEqual(login.EntryValue, abstractService.GetPreferences("UserID", string.Empty));
        }

        [Test]
        public void LoginNoSuccess()
        {
            var login = new LoginViewModel(connectionService, abstractService);
            Assert.IsNull(login.EntryValue);
            login.EntryValue = "0000";
            bool a = login.LoginCommand.CanExecute(null);
            Assert.IsFalse(a);
            login.LoginCommand.Execute(null);
        }

        [Test]
        public void LoginAlreadyLoggedIn()
        {
            abstractService.SetPreferences("UserID", "1234");
            var login = new LoginViewModel(connectionService, abstractService);
            // TODO
            var user = new User("1234");
            Assert.AreEqual("1234", user.UID);
        }


    }
}