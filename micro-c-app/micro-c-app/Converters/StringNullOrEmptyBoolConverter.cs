using System;
using System.Globalization;
using Xamarin.Forms;

namespace micro_c_app
{
    public class StringNullOrEmptyBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter is string paramString && bool.TryParse(paramString, out bool flip) && flip)
            {
                return string.IsNullOrWhiteSpace(value as string);
            }
            return !string.IsNullOrWhiteSpace(value as string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}