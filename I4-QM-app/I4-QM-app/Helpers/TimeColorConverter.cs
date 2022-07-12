using System;
using System.Globalization;
using Xamarin.Forms;

namespace I4_QM_app.Helpers
{
    /// <summary>
    /// Time-Color-Conversion.
    /// </summary>
    public class TimeColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a time into a color.
        /// </summary>
        /// <param name="value">datetime.</param>
        /// <param name="targetType">targetType.</param>
        /// <param name="parameter">parameter.</param>
        /// <param name="culture">culture.</param>
        /// <returns>Associated color.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                DateTime due = DateTime.Parse(value.ToString());
                double hoursTo = due.Subtract(DateTime.Now).TotalHours;

                if (hoursTo < 24)
                {
                    return TryGetColor("AccentRed", Color.Red);
                }
                else if (hoursTo > 24 && hoursTo < 72)
                {
                    return TryGetColor("AccentYellow", Color.Yellow);
                }
                else
                {
                    return TryGetColor("Primary", Color.LightGreen);
                }
            }

            return TryGetColor("Secondary", Color.Gray);
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
