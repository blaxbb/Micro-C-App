using micro_c_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RemindersPage : ContentPage
    {
        public RemindersPage()
        {
            InitializeComponent();
        }

        Reminder previousSelection;
        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var newItem = e.Item;

            if (previousSelection == newItem)
            {
                listView.SelectedItem = null;
                previousSelection = null;
            }
            else
            {
                previousSelection = newItem as Reminder;
            }
        }
    }
}