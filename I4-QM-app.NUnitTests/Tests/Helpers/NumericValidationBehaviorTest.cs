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
            // TODO
        }

        [Test]
        public void OnDetach()
        {
            var e = new Entry();
            var a = new NumericValidationBehaviorTestable();
            a.OnDetachingFromTestable(e);
            Assert.IsNotNull(a);
            // TODO
        }

        [Test]
        public void EntryChanged()
        {
            var e = new Entry();
            var a = new NumericValidationBehaviorTestable();
            a.OnAttachedToTestable(e);
            Assert.IsNotNull(a);

            // TODO
            e.Text = "1";
            Assert.AreEqual("1", e.Text);
        }
    }
}
