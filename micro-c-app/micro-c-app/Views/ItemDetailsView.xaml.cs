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
                view.SetSpecs();
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
            SpecsGrid.Children.Clear();
            SpecsGrid.RowDefinitions.Clear();
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

                        AddStripedBackground(SpecsGrid, stripeColor, row);
                        
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

                        SpecsGrid.Children.Add(name);
                        SpecsGrid.Children.Add(price);
                        Grid.SetColumn(name, 0);
                        Grid.SetColumn(price, 1);
                        Grid.SetRow(name, row);
                        Grid.SetRow(price, row);

                        SpecsGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
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
                            Margin = new Thickness(10)
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
        }
    }
}