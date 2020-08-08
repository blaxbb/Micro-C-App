using micro_c_app.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace micro_c_app.Views
{
    public partial class SearchPage : ContentPage
    {
        HttpClient client;
        public SearchPage()
        {
            client = new HttpClient();
            InitializeComponent();
            StorePicker.SelectedIndexChanged += StorePicker_SelectedIndexChanged;

            if(BindingContext is SearchViewModel vm)
            {
                vm.Navigation = Navigation;
            }
        }

        private void StorePicker_SelectedIndexChanged(object sender, EventArgs e)
        {

            var storeId = SearchViewModel.Stores[(string)StorePicker.SelectedItem];
            Preferences.Set(SearchViewModel.PREF_SELECTED_STORE, storeId);
        }
    }
}