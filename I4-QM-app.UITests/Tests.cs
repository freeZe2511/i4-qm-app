using NUnit.Framework;
using System.Linq;
using Xamarin.UITest;

namespace I4_QM_app.UITests
{
    [TestFixture(Platform.Android)]
    //[TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void LoginDisabled()
        {
            app.WaitForElement(c => c.Marked("LoginEntry"));
            app.EnterText(c => c.Marked("LoginEntry"), "12");
            app.Tap("LoginButton");
            var res = app.Query(x => x.Marked("LoginButton")).FirstOrDefault().Enabled;
            Assert.IsFalse(res);

        }

        [Test]
        public void SuccessfulLogin()
        {
            app.WaitForElement(c => c.Marked("LoginEntry"));
            app.EnterText(c => c.Marked("LoginEntry"), "1234");

            app.Tap("LoginButton");

        }
    }
}
