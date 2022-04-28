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
    public partial class ProductView : ContentView
    {
        public static readonly BindableProperty ItemProperty = BindableProperty.Create(nameof(Item), typeof(Item), typeof(ItemDetailsView), null, propertyChanged: ItemChanged);
        public Item Item { get => (Item)GetValue(ItemProperty); set => SetValue(ItemProperty, value); }

        public static readonly BindableProperty FastViewProperty = BindableProperty.Create(nameof(FastView), typeof(bool), typeof(ItemDetailsView), null);
        public bool FastView { get => (bool)GetValue(FastViewProperty); set => SetValue(FastViewProperty, value); }

        public ProductView()
        {
            InitializeComponent();
        }

        private static void ItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ProductView view && view.BindingContext is ProductViewModel vm)
            {
                vm.Item = newValue as Item;
                if (vm.Item != null)
                {
                    vm.Item.PropertyChanged += view.Item_PropertyChanged;
                }
                vm.FastView = view.FastView;
                view.StripeStacks();
            }
        }

        public void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            StripeStacks();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if(width > height)
            {
                priceStack.Orientation = StackOrientation.Vertical;
                priceStack.Children[0].HorizontalOptions = LayoutOptions.Start;
                priceStack.Children[1].HorizontalOptions = LayoutOptions.Start;
                priceStack.Children[2].HorizontalOptions = LayoutOptions.Start;

                Grid.SetColumnSpan(priceStack, 1);

                picture.IsVisible = false;

                detailGrid.Children.Add(tabButtons);
                Grid.SetRow(tabButtons, 1);
                Grid.SetColumn(tabButtons, 1);
                Grid.SetRowSpan(tabButtons, 2);

                detailGrid.RowDefinitions.Last().Height = GridLength.Star;
                mainGrid.RowDefinitions[0].Height = GridLength.Star;
                mainGrid.RowDefinitions[1].Height = 0;
                mainGrid.RowDefinitions[2].Height = 0;

                var tabs = new View[] { inventoryTab, clearanceTab, planTab, specsTab };
                foreach (var tab in tabs)
                {
                    detailGrid.Children.Add(tab);
                    Grid.SetRow(tab, 3);
                    Grid.SetColumn(tab, 1);
                    Grid.SetRowSpan(tab, 3);
                }
            }
            else
            {
                priceStack.Orientation = StackOrientation.Horizontal;
                priceStack.Children[0].HorizontalOptions = LayoutOptions.StartAndExpand;
                priceStack.Children[1].HorizontalOptions = LayoutOptions.CenterAndExpand;
                priceStack.Children[2].HorizontalOptions = LayoutOptions.EndAndExpand;

                Grid.SetColumnSpan(priceStack, 2);

                picture.IsVisible = true;

                mainGrid.Children.Add(tabButtons);
                Grid.SetRow(tabButtons, 1);
                Grid.SetColumn(tabButtons, 0);
                Grid.SetRowSpan(tabButtons, 1);

                detailGrid.RowDefinitions.Last().Height = GridLength.Auto;
                mainGrid.RowDefinitions[0].Height = GridLength.Auto;
                mainGrid.RowDefinitions[1].Height = GridLength.Auto;
                mainGrid.RowDefinitions[2].Height = GridLength.Star;

                var tabs = new View[] { inventoryTab, clearanceTab, planTab, specsTab };
                foreach(var tab in tabs)
                {
                    mainGrid.Children.Add(tab);
                    Grid.SetRow(tab, 2);
                    Grid.SetColumn(tab, 0);
                    Grid.SetRowSpan(tab, 1);
                }
            }
            if (width > 0 && height > 0)
            {
                InvalidateMeasure();
            }
        }

        private void StripeStacks()
        {
            var stripeColor = Application.Current.UserAppTheme == OSAppTheme.Dark ||
                              (Application.Current.UserAppTheme == OSAppTheme.Unspecified && Application.Current.RequestedTheme == OSAppTheme.Dark)
                              ? Color.FromHex("FF595959") : Color.LightGray;

            var stacks = new[] { SpecsStack, PlanStack, ClearanceStack, InventoryStack };
            foreach (var stack in stacks)
            {
                for (int i = 0; i < stack.Children.Count; i++)
                {
                    var child = stack.Children[i];
                    if (i % 2 == 0)
                    {
                        child.BackgroundColor = stripeColor;
                    }
                }
            }
        }
    }
}