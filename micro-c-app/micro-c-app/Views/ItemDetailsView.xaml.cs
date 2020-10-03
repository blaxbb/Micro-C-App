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
        }

        private static void ItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if(bindable is ItemDetailsView view && view.BindingContext is ItemDetailsViewViewModel vm)
            {
                vm.Item = newValue as Item;
                view.UpdatePlansAndSpecs();
            }
        }
        
        public void UpdatePlansAndSpecs()
        {
            SetPlanItems();
            SetSpecItems();
        }

        private static void AddSpacer(StackLayout stack, Color color)
        {
            stack.Children.Add(new BoxView() { Color = color, WidthRequest = 100, HeightRequest = 2, HorizontalOptions = LayoutOptions.FillAndExpand });
        }

        private void SetPlanItems()
        {
            PlansStackLayout.Children.Clear();
            if (BindingContext is ItemDetailsViewViewModel vm && vm.Item != null)
            {
                if (vm.Item.Plans != null)
                {

                    foreach (var plan in vm.Item.Plans)
                    {
                        AddSpacer(PlansStackLayout, Color.LightGray);
                        var stack = new StackLayout() { Orientation = StackOrientation.Horizontal };
                        stack.Children.Add(new SelectableLabel() { Text = plan.Name, HorizontalOptions = LayoutOptions.StartAndExpand, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start });
                        stack.Children.Add(new SelectableLabel() { Text = $"${plan.Price.ToString("#0.00")}", HorizontalOptions = LayoutOptions.End, WidthRequest = 100, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.End });

                        PlansStackLayout.Children.Add(stack);
                    }
                }
            }
        }

        private void SetSpecItems()
        {
            SpecsStackLayout.Children.Clear();
            if (BindingContext is ItemDetailsViewViewModel vm && vm.Item != null)
            {
                if (vm.Item.Specs != null)
                {
                    foreach (var spec in vm.Item.Specs)
                    {
                        AddSpacer(SpecsStackLayout, Color.LightGray);
                        var stack = new StackLayout() { Orientation = StackOrientation.Horizontal };
                        stack.Children.Add(new SelectableLabel()
                        {
                            Text = spec.Key,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.Start,
                            MinimumWidthRequest = 200,
                            WidthRequest = 200
                        });
                        stack.Children.Add(new SelectableLabel()
                        {
                            Text = spec.Value,
                            HorizontalOptions = LayoutOptions.Fill,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.Start
                        });
                        SpecsStackLayout.Children.Add(stack);
                    }
                }
            }
        }
    }
}