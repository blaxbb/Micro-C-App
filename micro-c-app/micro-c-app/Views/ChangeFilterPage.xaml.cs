using micro_c_app.ViewModels;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangeFilterPage : ContentPage
    {
        public Dictionary<string, List<string>> SpecFilters = new Dictionary<string, List<string>>();
        private static string[] ignore = new string[]
        {
            "SKU",
            "Mfr Part#",
            "UPC",
            "What's in the Box",
            "Labor",
            "Parts"
        };
        public ChangeFilterPage()
        {
            SpecFilters = new Dictionary<string, List<string>>();
            InitializeComponent();
        }

        public ChangeFilterPage(List<Item> items, Dictionary<string, List<string>> specFilters)
        {
            SpecFilters = specFilters;
            InitializeComponent();
            SetupSpecFilters(items.Select(i => i.CloneAndResetQuantity()).ToList());
        }

        private void SetupSpecFilters(List<Item> items)
        {
            Dictionary<string, StackLayout> filterCategories = new Dictionary<string, StackLayout>();
            Dictionary<string, List<string>> allFilterOptions = new Dictionary<string, List<string>>();

            StackLayout allFiltersStack = new StackLayout()
            {
                Orientation = StackOrientation.Vertical
            };

            foreach (var item in items)
            {
                AddFilterItem("Brand", item.Brand, allFiltersStack, filterCategories, allFilterOptions);

                foreach(var spec in item.Specs)
                {
                    var field = spec.Key;
                    var value = spec.Value;

                    AddFilterItem(field, value, allFiltersStack, filterCategories, allFilterOptions);
                }
            }

            scrollView.Content = allFiltersStack;
        }

        private void AddFilterItem(string field, string value, StackLayout allFiltersStack, Dictionary<string, StackLayout> filterCategories, Dictionary<string, List<string>> allFilterOptions)
        {
            if (ignore.Contains(field))
            {
                return;
            }

            if (!filterCategories.ContainsKey(field))
            {
                var filter = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical
                };

                filter.Children.Add(new Label()
                {
                    Text = field,
                    Padding = new Thickness(10),
                    InputTransparent = true
                });
                var optionsStack = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    IsVisible = false,
                    Padding = new Thickness(20, 10)
                };
                filter.Children.Add(optionsStack);
                filter.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(() => { optionsStack.IsVisible = !optionsStack.IsVisible; })
                });
                filterCategories[field] = optionsStack;
                allFilterOptions[field] = new List<string>();
                allFiltersStack.Children.Add(filter);
            }

            var stack = filterCategories[field];
            foreach (var unique in AddUnique(allFilterOptions[field], value.Split('\n')))
            {
                var internalStack = new StackLayout() { Orientation = StackOrientation.Horizontal };
                internalStack.Children.Add(new Label()
                {
                    Text = unique,
                    InputTransparent = true,
                    HorizontalOptions = LayoutOptions.StartAndExpand
                });
                var s = new Switch();
                if (SpecFilters.ContainsKey(field) && SpecFilters[field].Contains(unique))
                {
                    s.IsToggled = true;
                }
                s.Toggled += (sender, args) => { ToggleFilter(field, unique); };

                internalStack.Children.Add(s);

                stack.Children.Add(internalStack);
            }
        }

        private IEnumerable<string> AddUnique(List<string> source, IEnumerable<string> items)
        {
            foreach(var i in items)
            {
                if (!source.Contains(i))
                {
                    source.Add(i);
                    yield return i;
                }
            }
        }

        private void ToggleFilter(string filterName, string option)
        {
            if (SpecFilters.ContainsKey(filterName))
            {
                var filter = SpecFilters[filterName];
                if (filter.Contains(option))
                {
                    filter.Remove(option);
                }
                else
                {
                    filter.Add(option);
                }
            }
            else
            {
                SpecFilters.Add(filterName, new List<string>() { option });
            }
        }
    }
}