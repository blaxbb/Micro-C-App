using micro_c_app.ViewModels;
using MicroCLib.Models;
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
    public partial class ItemDetailsView : ContentView
    {
        public static readonly BindableProperty ItemProperty = BindableProperty.Create(nameof(Item), typeof(Item), typeof(ItemDetailsView), null, propertyChanged: ItemChanged);
        public Item Item { get => (Item)GetValue(ItemProperty); set => SetValue(ItemProperty, value); }

        public static readonly BindableProperty FastViewProperty = BindableProperty.Create(nameof(FastView), typeof(bool), typeof(ItemDetailsView), null);
        public bool FastView { get => (bool)GetValue(FastViewProperty); set => SetValue(FastViewProperty, value); }

        public ItemDetailsView()
        {
            this.BindingContext = new ItemDetailsViewViewModel();
            InitializeComponent();
            MessagingCenter.Subscribe<SettingsPageViewModel>(this, SettingsPageViewModel.SETTINGS_UPDATED_MESSAGE, SettingsUpdated);
        }

        private void SettingsUpdated(SettingsPageViewModel obj)
        {
            SetSpecs();
        }

        private static void ItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ItemDetailsView view && view.BindingContext is ItemDetailsViewViewModel vm)
            {
                vm.Item = newValue as Item;
                vm.FastView = view.FastView;
                view.SetSpecs();
                view.SetClearance();
            }
        }

        private static void AddStripedBackground(Grid grid, Color color, int row)
        {
            var background = new BoxView() { Color = row % 2 == 0 ? color : Color.Transparent, WidthRequest = 100, HeightRequest = 1, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
            grid.Children.Add(background);
            Grid.SetRow(background, row);
            Grid.SetColumnSpan(background, grid.ColumnDefinitions.Count);
        }

        private void SetSpecs()
        {
            if(SpecsGrid == null)
            {
                return;
            }

            /*
             * Performance on repeatedly adding things to a grid is awful, since
             * the grid needs to resize elements on each add.  Detaching from parent,
             * and then re-attaching at the end skips all of those resize calls.
             */
            Grid? parentGrid = SpecsGrid.Parent as Grid;
            if(parentGrid != null)
            {
                parentGrid.Children.Remove(SpecsGrid);
                parentGrid.Children.Remove(PlanGrid);
            }

            SpecsGrid.Children.Clear();
            SpecsGrid.RowDefinitions.Clear();

            PlanGrid.Children.Clear();
            PlanGrid.RowDefinitions.Clear();

            var stripeColor = Application.Current.UserAppTheme == OSAppTheme.Dark ||
                              (Application.Current.UserAppTheme == OSAppTheme.Unspecified && Application.Current.RequestedTheme == OSAppTheme.Dark)
                              ? Color.FromHex("FF595959") : Color.LightGray;

            //column row defs
            if (BindingContext is ItemDetailsViewViewModel vm && vm.Item?.Specs != null)
            {
                if (vm.Item?.Plans != null)
                {
                    for (int i = 0; i < vm.Item.Plans.Count; i++)
                    {
                        var row = i;
                        var plan = vm.Item.Plans[i];

                        AddStripedBackground(PlanGrid, stripeColor, row);
                        
                        var name = new Label()
                        {
                            Text = plan.Name,
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            HorizontalTextAlignment = TextAlignment.Start,
                            VerticalTextAlignment = TextAlignment.Center,
                            LineBreakMode = LineBreakMode.WordWrap,
                            Margin = new Thickness(10)
                        };
                        var price = new Label()
                        {
                            Text = $"${plan.Price.ToString("#0.00")}",
                            HorizontalOptions = LayoutOptions.EndAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.End,
                            LineBreakMode = LineBreakMode.WordWrap,
                            Margin = new Thickness(10)
                        };

                        PlanGrid.Children.Add(name);
                        PlanGrid.Children.Add(price);
                        Grid.SetColumn(name, 0);
                        Grid.SetColumn(price, 1);
                        Grid.SetRow(name, row);
                        Grid.SetRow(price, row);

                        PlanGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    }
                }
                if (vm.Item?.Specs != null)
                {
                    int startingRow = SpecsGrid.RowDefinitions.Count;

                    for (int i = 0; i < vm.Item.Specs.Count; i++)
                    {
                        var row = startingRow + i;
                        AddStripedBackground(SpecsGrid, stripeColor, row);

                        var spec = vm.Item.Specs.ElementAt(i);
                        var name = new Label()
                        {
                            Text = spec.Key,
                            HorizontalOptions = LayoutOptions.StartAndExpand,
                            HorizontalTextAlignment = TextAlignment.Start,
                            VerticalTextAlignment = TextAlignment.Center,
                            LineBreakMode = LineBreakMode.WordWrap,
                            Margin = new Thickness(10)
                        };
                        var val = new Label()
                        {
                            Text = spec.Value,
                            HorizontalOptions = LayoutOptions.EndAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.End,
                            LineBreakMode = LineBreakMode.WordWrap,
                            Margin = new Thickness(10),
                        };

                        SpecsGrid.Children.Add(name);
                        SpecsGrid.Children.Add(val);
                        Grid.SetColumn(name, 0);
                        Grid.SetColumn(val, 1);
                        Grid.SetRow(name, row);
                        Grid.SetRow(val, row);
                        SpecsGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    }
                }
            }

            if (parentGrid != null)
            {
                parentGrid.Children.Add(PlanGrid);
                parentGrid.Children.Add(SpecsGrid);
            }
        }

        private void PopulateGrid(Grid grid, IEnumerable<IEnumerable<View>> views)
        {
            if(grid == null || views == null)
            {
                return;
            }

            Grid? parent = null;
            if (grid.Parent is Grid p)
            {
                parent = p;
                parent.Children.Remove(grid);
            }

            grid.RowDefinitions.Clear();
            grid.Children.Clear();

            var stripeColor = Application.Current.UserAppTheme == OSAppTheme.Dark ||
                  (Application.Current.UserAppTheme == OSAppTheme.Unspecified && Application.Current.RequestedTheme == OSAppTheme.Dark)
                  ? Color.FromHex("FF595959") : Color.LightGray;

            int rowIndex = 0;
            foreach(var row in views)
            {
                int viewIndex = 0;
                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                AddStripedBackground(grid, stripeColor, rowIndex);
                foreach (var view in row)
                {
                    grid.Children.Add(view);
                    Grid.SetColumn(view, viewIndex);
                    Grid.SetRow(view, rowIndex);
                    viewIndex++;
                }
                rowIndex++;
            }

            if(parent != null)
            {
                parent.Children.Add(grid);
            }
        }

        private void SetClearance()
        {
            if(Item == null || Item.ClearanceItems == null)
            {
                return;
            }

            List<List<View>> views = new List<List<View>>();
            foreach (var clearance in Item.ClearanceItems)
            {
                views.Add(new List<View>()
                {
                    new Label()
                    {
                        Text = clearance.State,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.WordWrap,
                        Margin = new Thickness(10)
                    },
                    new Label()
                    {
                        Text = clearance.Price.ToString(),
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.End,
                        LineBreakMode = LineBreakMode.WordWrap,
                        Margin = new Thickness(10),
                    }
                });
            }
            PopulateGrid(ClearanceGrid, views);
        }
    }
}