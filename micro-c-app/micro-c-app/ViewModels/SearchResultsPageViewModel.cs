using micro_c_app.Models;
using System.Collections.ObjectModel;

namespace micro_c_app.ViewModels
{
    public class SearchResultsPageViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; }

        public SearchResultsPageViewModel()
        {
            Title = "Search";
            Items = new ObservableCollection<Item>();
        }
    }
}