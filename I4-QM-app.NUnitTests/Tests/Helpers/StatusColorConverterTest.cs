using I4_QM_app.Helpers;
using NUnit.Framework;
using Xamarin.Forms;

namespace I4_QM_app.NUnitTests.Tests.Helpers
{
    internal class StatusColorConverterTest
    {
        [Test]
        public void TestStatusConvertOpen()
        {
            var a = new StatusColorConverter();
            Assert.AreEqual(Color.Red, a.Convert("Open", null, null, null));
        }

        [Test]
        public void TestStatusConvertMixed()
        {
            var a = new StatusColorConverter();
            Assert.AreEqual(Color.Yellow, a.Convert("Mixed", null, null, null));
        }

        [Test]
        public void TestStatusConvertRated()
        {
            var a = new StatusColorConverter();
            Assert.AreEqual(Color.LightGreen, a.Convert("Rated", null, null, null));
        }

        [Test]
        public void TestStatusConvertNone()
        {
            var a = new StatusColorConverter();
            Assert.AreEqual(Color.Black, a.Convert("", null, null, null));
        }

        [Test]
        public void TestStatusConvertBack()
        {
            var a = new StatusColorConverter();
            Assert.AreEqual("test", a.ConvertBack("test", null, null, null));
        }

    }
}
