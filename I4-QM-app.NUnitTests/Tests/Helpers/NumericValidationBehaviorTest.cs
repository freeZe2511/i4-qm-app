using NUnit.Framework;
using Xamarin.Forms;

namespace I4_QM_app.NUnitTests.Tests.Helpers
{
    internal class NumericValidationBehaviorTest
    {
        [Test]
        public void OnAttach()
        {
            var e = new Entry();
            var a = new NumericValidationBehaviorTestable();
            a.OnAttachedToTestable(e);
            Assert.IsNotNull(a);
        }
    }
}
