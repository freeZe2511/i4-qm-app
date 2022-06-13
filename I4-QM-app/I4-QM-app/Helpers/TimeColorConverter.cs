using System;
using System.Globalization;
using Xamarin.Forms;

namespace I4_QM_app.Helpers
{
    public class TimeColorConverter : IValueConverter
    {

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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        internal static Color TryGetColor(string key, Color fallback)
        {
            Application.Current.Resources.TryGetValue(key, out var color);

            return color as Color? ?? fallback;
        }
    }
}
