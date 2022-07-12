using Xamarin.Forms;

namespace I4_QM_app.Helpers
{
    /// <summary>
    /// Extends MenuItem with an icon.
    /// </summary>
    public class MenuItemIconFont : MenuItem
    {
        /// <summary>
        /// Bindable Icon.
        /// </summary>
        public static readonly BindableProperty IconGlyphProperty = BindableProperty.Create(nameof(IconGlyphProperty), typeof(string), typeof(FlyoutItemIconFont), string.Empty);

        /// <summary>
        /// Gets or sets icon string.
        /// </summary>
        public string IconGlyph
        {
            get { return (string)GetValue(IconGlyphProperty); }
            set { SetValue(IconGlyphProperty, value); }
        }
    }
}
