﻿using micro_c_app.Models;
using micro_c_app.Views;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public ObservableCollection<Item> Items { get => items; set => SetProperty(ref items, value); }
        public ObservableCollection<Item> FilteredItems { get => filteredItems; set => SetProperty(ref filteredItems, value); }

        public string SearchQuery { get; set; }
        public string StoreID { get; set; }
        public string CategoryFilter { get; set; }
        public OrderByMode OrderBy { get; set; }

        public ICommand ChangeOrderBy { get; }

        HttpClient client;
        private int itemThreshold = 5;
        private int totalResults;
        private int page = 1;
        private bool enhancedSearch;
        private bool loadingNextResults;
        private EnhancedSearchSettings searchSettings;
        private ObservableCollection<Item> items;
        private ObservableCollection<Item> filteredItems;
        private Dictionary<string, List<string>> specFilters;
        public const int RESULTS_PER_PAGE = 96;
        public int ItemThreshold { get => itemThreshold; set => SetProperty(ref itemThreshold, value); }
        public ICommand LoadMore { get; }
        public int Page { get => page; set => SetProperty(ref page, value); }
        public int TotalPages => (int)Math.Ceiling((double)totalResults / RESULTS_PER_PAGE);
        public int TotalResults { get => totalResults; set => SetProperty(ref totalResults, value); }
        public bool EnhancedSearch { get => enhancedSearch; set { SetProperty(ref enhancedSearch, value); if (value) { ItemThreshold = -1; } else { ItemThreshold = 10; } } }

        public bool LoadingNextResults { get => loadingNextResults; set => SetProperty(ref loadingNextResults, value); }

        public EnhancedSearchSettings SearchSettings { get => searchSettings; set => SetProperty(ref searchSettings, value); }
        public ICommand ToggleSortDirection { get; }
        public ICommand ChangeFilter { get; }
        public Dictionary<string, List<string>> SpecFilters { get => specFilters; set => SetProperty(ref specFilters, value); }

        public SearchResultsPageViewModel()
        {
            Title = "Search";
            client = new HttpClient();
            Items = new ObservableCollection<Item>();
            FilteredItems = new ObservableCollection<Item>();
            SearchSettings = new EnhancedSearchSettings();
            SpecFilters = new Dictionary<string, List<string>>();

            LoadMore = new Command(async () =>
            {
                if (loadingNextResults)
                {
                    return;
                }

                if (page < TotalPages)
                {
                    page++;
                    loadingNextResults = true;
                    await LoadQuery();
                    loadingNextResults = false;
                }
            });

            ChangeOrderBy = new Command(async () =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    string[] options;
                    if (EnhancedSearch)
                    {
                        options = EnhancedSearchSettings.GetOptions(Items);
                    }
                    else
                    {
                        options = Enum.GetNames(typeof(OrderByMode));
                    }
                    var result = await Shell.Current.DisplayActionSheet("Order Mode", "Cancel", null, options);
                    if (result != null && result != "Cancel")
                    {
                        if (EnhancedSearch)
                        {
                            SearchSettings.Field = result;
                            SearchSettings.Ascending = true;
                            Items = new ObservableCollection<Item>(SearchSettings.GetSorted(Items));
                            FilteredItems = new ObservableCollection<Item>(Filter(Items.AsEnumerable()));
                        }
                        else
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
                    }
                });
            });

            ChangeFilter = new Command(async () =>
            {
                var page = new ChangeFilterPage(Items.ToList(), SpecFilters);
                await Shell.Current.Navigation.PushAsync(page);
                page.Disappearing += ((sender, args) =>
                {
                    SpecFilters = page.SpecFilters;
                    FilteredItems = new ObservableCollection<Item>(Filter(Items.AsEnumerable()));
                });
            });

            ToggleSortDirection = new Command(() =>
            {
                if (SearchSettings != null)
                {
                    SearchSettings.Ascending = !SearchSettings.Ascending;
                    Items = new ObservableCollection<Item>(SearchSettings.GetSorted(Items));
                    FilteredItems = new ObservableCollection<Item>(Filter(items.AsEnumerable()));
                }
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
            catch (Exception e)
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
            FilteredItems = new ObservableCollection<Item>(Items);
            TotalResults = results.TotalResults;
            page = results.Page;
        }

        private IEnumerable<Item> Filter(IEnumerable<Item> items)
        {
            return items.Where(item =>
            {
                foreach (var specFilter in SpecFilters)
                {
                    if (specFilter.Value.Count == 0)
                    {
                        continue;
                    }

                    string value;
                    switch (specFilter.Key)
                    {
                        case "Brand":
                            value = item.Brand;
                            break;
                        default:
                            value = item.Specs.ContainsKey(specFilter.Key) ? item.Specs[specFilter.Key] : null;
                            break;
                    }

                    if (CheckFilter(specFilter.Value, value))
                    {
                        return false;
                    }
                }

                return true;
            });
        }

        private bool CheckFilter(List<string> filters, string value)
        {
            if(filters.Count() == 0)
            {
                return false;
            }

            if (filters.Contains(value))
            {
                return false;
            }

            return true;
        }
    }

    public class EnhancedSearchSettings : NotifyPropertyChangedItem
    {
        private bool ascending;
        private string field;

        public bool Ascending { get => ascending; set => SetProperty(ref ascending, value); }

        public string Field { get => field; set => SetProperty(ref field, value); }

        public static string[] GetOptions(IEnumerable<Item> items)
        {
            var intrinsic = new string[]
            {
                "Price",
                "Stock",
                "Brand"
            };

            string[] ignore = new string[]
            {
                "SKU",
                "Mfr Part#",
                "UPC",
                "What's in the Box",
                "Labor",
                "Parts"
            };

            return intrinsic.Concat(items.SelectMany(i => i.Specs.Keys.ToList()).Distinct().Where(s => !ignore.Contains(s))).ToArray();
        }

        public IEnumerable<Item> GetSorted(IEnumerable<Item> items)
        {
            switch (Field)
            {
                case null:
                case "Price":
                    return Ascending ? items.OrderBy(i => i.Price) : items.OrderByDescending(i => i.Price);
                case "Stock":
                    return Ascending ? items.OrderBy(i => ParseStock(i.Stock)) : items.OrderByDescending(i => ParseStock(i.Stock));
                case "Brand":
                    return Ascending ? items.OrderBy(i => i.Brand) : items.OrderByDescending(i => i.Brand);
                default:
                    return Ascending ? items.OrderBy(i => i.Specs.ContainsKey(Field) ? i.Specs[Field] : "") : items.OrderByDescending(i => i.Specs.ContainsKey(Field) ? i.Specs[Field] : "");
            }

            return items;
        }

        private float ParseStock(string s)
        {
            bool qtyPlus = false;
            if(s.Length > 0 && s[s.Length - 1] == '+')
            {
                qtyPlus = true;
            }
            var match = Regex.Match(s, "([\\d\\.]*)");
            if (match.Success)
            {
                if(float.TryParse(match.Groups[1].Value, out float f))
                {
                    if(qtyPlus)
                    {
                        return f + 1;
                    }
                    return f;
                }
            }

            return 0;
        }
    }
}