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
    public partial class SearchResultsPage : ContentPage
    {
        public event EventHandler<ItemTappedEventArgs> ItemTapped;
        public bool AutoPop { get; set; }
        public SearchResultsPage()
        {
            InitializeComponent();
            ItemsList.ItemTapped += ItemsList_ItemTapped;
        }

        private void ItemsList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ItemTapped?.Invoke(sender, e);
            if (AutoPop)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.Navigation.PopAsync();
                });
            }
        }
    }
}