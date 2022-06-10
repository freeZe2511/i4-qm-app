using System;
using System.Globalization;
using Xamarin.Forms;

namespace I4_QM_app.Helpers
{
    public class StatusColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "open":
                    return TryGetColor("AccentRed", Color.Red);
                case "mixed":
                    return TryGetColor("AccentYellow", Color.Yellow);
                case "rated":
                    return TryGetColor("AccentBlueDark", Color.DarkBlue);
                default: return Color.Black;
            }

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
