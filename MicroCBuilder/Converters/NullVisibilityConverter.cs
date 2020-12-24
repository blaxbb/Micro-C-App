using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MicroCBuilder.Converters
{
    public class NullVisibilityConverter : IValueConverter
    {
        public bool NullIsCollapsed { get; set; } = true;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is string s)
            {
                if(string.IsNullOrWhiteSpace(s))
                {
                    return NullIsCollapsed ? Visibility.Collapsed : Visibility.Visible;
                }

                return NullIsCollapsed ? Visibility.Visible : Visibility.Collapsed;
            }    

            if(value is null)
            {
                return  NullIsCollapsed ? Visibility.Collapsed : Visibility.Visible;
            }

            return NullIsCollapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
