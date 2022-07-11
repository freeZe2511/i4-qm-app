using I4_QM_app.Helpers;
using Xamarin.Forms;

namespace I4_QM_app.NUnitTests.Tests.Helpers
{
    internal class NumericValidationBehaviorTestable : NumericValidationBehavior
    {
        public void OnAttachedToTestable(Entry entry)
        {
            base.OnAttachedTo(entry);
        }

        public void OnDetachingFromTestable(Entry entry)
        {
            base.OnDetachingFrom(entry);
        }
    }
}
