using I4_QM_app.NUnitTests.MockServices;
using I4_QM_app.Services;
using I4_QM_app.Services.Abstract;
using I4_QM_app.ViewModels;
using NUnit.Framework;

namespace I4_QM_app.NUnitTests
{
    public class Login
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
            bool a = login.LoginCommand.CanExecute(null);
            Assert.IsFalse(a);
            login.EntryValue = "1234";
            bool b = login.LoginCommand.CanExecute(null);
            Assert.IsTrue(b);
            // TODO
            //login.LoginCommand.Execute(null);
            //Assert.AreEqual(login.EntryValue, abstractService.GetPreferences("UserID", string.Empty));
        }

        [Test]
        public void LoginNoSuccess()
        {
            var login = new LoginViewModel(connectionService, abstractService);
            Assert.IsNull(login.EntryValue);
            login.EntryValue = "abc123";
            bool a = login.LoginCommand.CanExecute(null);
            Assert.IsFalse(a);
        }


    }
}