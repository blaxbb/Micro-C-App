using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using System.Linq;

namespace micro_c_app.Themes
{
    public class StripedRowDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EvenRow { get; set; }
        public DataTemplate OddRow { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if(container is ListView listView)
            {
                var items = listView.ItemsSource.Cast<object>().ToList();
                var index = items.IndexOf(item);
                return index % 2 == 0 ? EvenRow : OddRow;
            }

            return EvenRow;
        }
    }
}
