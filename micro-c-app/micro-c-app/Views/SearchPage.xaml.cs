using micro_c_app.ViewModels;
using System.Net.Http;
using Xamarin.Forms;

namespace micro_c_app.Views
{
    public partial class SearchPage : ContentPage
    {
        HttpClient client;
        public SearchPage()
        {
            client = new HttpClient();
            InitializeComponent();

            if (BindingContext is SearchViewModel vm)
            {
                vm.Navigation = Navigation;
            }
        }

        //private void StorePicker_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //    var storeId = SearchViewModel.Stores[(string)StorePicker.SelectedItem];
        //    Preferences.Set(SearchViewModel.PREF_SELECTED_STORE, storeId);
        //}
    }
}