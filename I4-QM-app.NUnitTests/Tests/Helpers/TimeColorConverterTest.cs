using I4_QM_app.Helpers;
using NUnit.Framework;
using System;
using Xamarin.Forms;

namespace I4_QM_app.NUnitTests.Tests.Helpers
{
    internal class TimeColorConverterTest
    {
        [Test]
        public void TestTimeConvertRed()
        {
            var a = new TimeColorConverter();
            Assert.AreEqual(Color.Red, a.Convert(DateTime.Now.AddHours(1), null, null, null));
        }

        [Test]
        public void TestTimeConvertYellow()
        {
            var a = new TimeColorConverter();
            Assert.AreEqual(Color.Yellow, a.Convert(DateTime.Now.AddHours(36), null, null, null));
        }

        [Test]
        public void TestTimeConvertGreen()
        {
            var a = new TimeColorConverter();
            Assert.AreEqual(Color.LightGreen, a.Convert(DateTime.Now.AddHours(100), null, null, null));
        }

        [Test]
        public void TestTimeConvertNone()
        {
            var a = new TimeColorConverter();
            Assert.AreEqual(Color.Gray, a.Convert(null, null, null, null));
        }

        [Test]
        public void TestTimeConvertBack()
        {
            var a = new TimeColorConverter();
            Assert.AreEqual("test", a.ConvertBack("test", null, null, null));
        }
    }
}
