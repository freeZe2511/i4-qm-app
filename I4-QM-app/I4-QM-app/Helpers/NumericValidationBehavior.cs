using System.Linq;
using Xamarin.Forms;

namespace I4_QM_app.Helpers
{
    /// <summary>
    /// Behavior for numeric validation for entries.
    /// </summary>
    // https://stackoverflow.com/questions/44475667/is-it-possible-specify-xamarin-forms-entry-numeric-keyboard-without-comma-or-dec
    public class NumericValidationBehavior : Behavior<Entry>
    {
        /// <inheritdoc/>
        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        /// <inheritdoc/>
        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        /// <summary>
        /// Only valid digits in entry allowed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="args">Args.</param>
        private static void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                bool isValid = args.NewTextValue.ToCharArray().All(x => char.IsDigit(x));

                ((Entry)sender).Text = isValid ? args.NewTextValue : args.OldTextValue;
            }
        }
    }
}
