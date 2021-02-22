using micro_c_app.Models;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static micro_c_app.Views.SearchView;
using static MicroCLib.Models.Search;

namespace micro_c_app.ViewModels
{
    public class SearchResultsPageViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; }

        public string SearchQuery { get; set; }
        public string StoreID { get; set; }
        public string CategoryFilter { get; set; }
        public OrderByMode OrderBy { get; set; }

        public ICommand ChangeOrderBy { get; }

        HttpClient client;
        private int itemThreshold = 5;
        private int totalResults;
        private int page = 1;
        public const int RESULTS_PER_PAGE = 96;
        public int ItemThreshold { get => itemThreshold; set => SetProperty(ref itemThreshold, value); }
        public ICommand LoadMore { get; }
        public int Page { get => page; set => SetProperty(ref page, value); }
        public int TotalPages => (int)Math.Ceiling((double)totalResults / RESULTS_PER_PAGE);
        public int TotalResults { get => totalResults; set => SetProperty(ref totalResults, value); }

        public SearchResultsPageViewModel()
        {
            Title = "Search";
            client = new HttpClient();
            Items = new ObservableCollection<Item>();

            LoadMore = new Command(async () =>
            {
                if(page < TotalPages)
                {
                    page++;
                    await LoadQuery();
                }
            });

            ChangeOrderBy = new Command(async () =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var result = await Shell.Current.DisplayActionSheet("Order Mode", "Cancel", null, Enum.GetNames(typeof(OrderByMode)));
                    if (result != null && result != "Cancel")
                    {
                        if (Enum.TryParse<OrderByMode>(result, out var newMode))
                        {
                            if (OrderBy != newMode)
                            {
                                OrderBy = newMode;
                                Items.Clear();
                                page = 1;
                                await LoadQuery();
                            }
                        }
                    }
                });
            });
        }

        private async Task LoadQuery()
        {
            try
            {
                var result = await Search.LoadQuery(SearchQuery, StoreID, CategoryFilter, OrderBy, page);
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    TotalResults = result.TotalResults;
                    foreach (var item in result.Items)
                    {
                        Items.Add(item);
                    }
                });
            }
            catch (TaskCanceledException e)
            {
                AnalyticsService.Track("Search Submit Cancelled");
                //triggered by user input, do nothing
            }
            catch (OperationCanceledException e)
            {
                AnalyticsService.Track("Search Submit Cancelled");
                //triggered by user input, do nothing
            }
            catch(Exception e)
            {
                AnalyticsService.TrackError(e, SearchQuery);
                await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
            }
        }

        public void ParseResults(SearchResults results)
        {
            foreach (var i in results.Items)
            {
                Items.Add(i);
            }

            TotalResults = results.TotalResults;
            page = results.Page;
        }
    }
}