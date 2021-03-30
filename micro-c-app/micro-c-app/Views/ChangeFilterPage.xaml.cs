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
        private Dictionary<string, Label> FilterQtyLabels = new Dictionary<string, Label>();
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
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            Task.Run((() => SetupSpecFilters(items.Select(i => i.CloneAndResetQuantity()).ToList())));
            sw.Stop();
            Console.WriteLine($"ELAPSED: {sw.ElapsedMilliseconds}");
        }

        private async Task SetupSpecFilters(List<Item> items)
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
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                await Task.Delay(16);
                scrollView.Content = allFiltersStack;
                scrollView.ForceLayout();
            });
            
        }

        private void AddFilterItem(string field, string value, StackLayout allFiltersStack, Dictionary<string, StackLayout> filterCategories, Dictionary<string, List<string>> allFilterOptions)
        {
            if (ignore.Contains(field))
            {
                return;
            }

            if (!filterCategories.ContainsKey(field))
            {
                var stripeColor = Application.Current.UserAppTheme == OSAppTheme.Dark ||
                  (Application.Current.UserAppTheme == OSAppTheme.Unspecified && Application.Current.RequestedTheme == OSAppTheme.Dark)
                  ? Color.FromHex("FF595959") : Color.LightGray;

                var filterStack = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical
                };

                var filterLabelStack = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal
                };

                filterLabelStack.Children.Add(new Label()
                {
                    Text = field,
                    Padding = new Thickness(10),
                    InputTransparent = true,
                    HorizontalOptions = LayoutOptions.StartAndExpand
                });

                int count = SpecFilters.ContainsKey(field) ? SpecFilters[field].Count : 0;
                var filterQtyLabel = new Label()
                {
                    Text = count.ToString(),
                    Padding = new Thickness(10),
                    InputTransparent = true,
                    HorizontalOptions = LayoutOptions.End
                };
                FilterQtyLabels[field] = filterQtyLabel;
                filterLabelStack.Children.Add(filterQtyLabel);

                filterStack.Children.Add(filterLabelStack);

                var optionsStack = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    IsVisible = false,
                    Padding = new Thickness(20, 10)
                };
                filterStack.Children.Add(optionsStack);
                filterStack.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(() => {
                        optionsStack.IsVisible = !optionsStack.IsVisible;
                        if (optionsStack.IsVisible)
                        {
                            filterStack.BackgroundColor = stripeColor;
                        }
                        else
                        {
                            filterStack.BackgroundColor = Color.Transparent;
                        }

                        if (scrollView.Content is StackLayout root)
                        {
                            foreach (StackLayout toDisable in root.Children)
                            {
                                if (filterStack != toDisable)
                                {
                                    toDisable.BackgroundColor = Color.Transparent;
                                    var opts = toDisable.Children[1];
                                    opts.IsVisible = false;
                                }
                            }
                        }
                    })
                });
                filterCategories[field] = optionsStack;
                allFilterOptions[field] = new List<string>();
                allFiltersStack.Children.Add(filterStack);
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
                s.Toggled += (sender, args) => {
                    ToggleFilter(field, unique);
                    int count = SpecFilters.ContainsKey(field) ? SpecFilters[field].Count : 0;
                    FilterQtyLabels[field].Text = count.ToString();
                };

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