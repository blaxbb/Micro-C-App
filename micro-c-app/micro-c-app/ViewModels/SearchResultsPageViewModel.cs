using micro_c_app.Models;
using micro_c_app.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace micro_c_app.ViewModels
{
    public class SearchResultsPageViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; }
        public ICommand OnProductSelected { get; }
        public ICommand OnProductError { get; }

        public SearchResultsPageViewModel()
        {
            Title = "Search";
            Items = new ObservableCollection<Item>();

            OnProductSelected = new Command<Item>(async (Item item) =>
            {

            });

            OnProductError = new Command<string>(async (string message) =>
            {
                
            });
        }
    }
}