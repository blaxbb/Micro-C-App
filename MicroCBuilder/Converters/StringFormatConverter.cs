using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Data;

namespace MicroCBuilder.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public string? Format { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!string.IsNullOrWhiteSpace(Format))
            {
                parameter = Format;
            }

            if(parameter is string format)
            {
                if (!string.IsNullOrWhiteSpace(format))
                {
                    return string.Format(format, value);
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
