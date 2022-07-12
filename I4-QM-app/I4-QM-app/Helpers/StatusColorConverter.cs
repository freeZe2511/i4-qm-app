using System;
using System.Globalization;
using Xamarin.Forms;

namespace I4_QM_app.Helpers
{
    /// <summary>
    /// Status-Color-Conversion.
    /// </summary>
    public class StatusColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a status into a color.
        /// </summary>
        /// <param name="value">Status.</param>
        /// <param name="targetType">targetType.</param>
        /// <param name="parameter">parameter.</param>
        /// <param name="culture">culture.</param>
        /// <returns>Associated Color.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "Open":
                    return TryGetColor("AccentRed", Color.Red);
                case "Mixed":
                    return TryGetColor("AccentYellow", Color.Yellow);
                case "Rated":
                    return TryGetColor("Primary", Color.LightGreen);
                default: return Color.Black;
            }
        }

        /// <summary>
        /// Not implemented, only for interface.
        /// </summary>
        /// <param name="value">value.</param>
        /// <param name="targetType">targetType.</param>
        /// <param name="parameter">parameter.</param>
        /// <param name="culture">culture.</param>
        /// <returns>Value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        /// <summary>
        /// Get color with key from ressources.
        /// </summary>
        /// <param name="key">Color key.</param>
        /// <param name="fallback">Fallback color.</param>
        /// <returns>Either found color or fallback.</returns>
        internal static Color TryGetColor(string key, Color fallback)
        {
            try
            {
                Application.Current.Resources.TryGetValue(key, out var color);
                return color as Color? ?? fallback;
            }
            catch (Exception)
            {
                // empty
            }

            return fallback;
        }
    }
}
