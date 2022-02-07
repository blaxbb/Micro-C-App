using System;
using System.Globalization;
using Xamarin.Forms;

namespace micro_c_app
{
    public class NullBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter is string s && bool.TryParse(s, out bool b) && b)
            {
                return value == null;
            }
            else if(parameter is int i)
            {
                return i != 0;
            }
            else
            {
                return value != null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}