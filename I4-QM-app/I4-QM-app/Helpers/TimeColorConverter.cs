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
                Console.WriteLine(value.GetType());
            }

            //var diff = DateTime.Now;

            //if (value != null) diff = (DateTime)value - DateTime.Now;

            //Console.WriteLine(diff.ToString());

            return Color.Black;
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
