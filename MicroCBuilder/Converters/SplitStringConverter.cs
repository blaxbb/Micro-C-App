using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MicroCBuilder.Converters
{
    public class SplitStringConverter : IValueConverter
    {
        public string? Separator { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is string s)
            {
                var res = s.Split(Separator ?? "\n").Where(s => s != Separator && !string.IsNullOrWhiteSpace(s)).ToArray();
                return res;
            }
            return new string[0];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
