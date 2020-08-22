using micro_c_app.ViewModels;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailsPage : ContentPage
    {
        public ItemDetailsPage()
        {
            InitializeComponent();
            BindingContextChanged += ItemDetails_BindingContextChanged;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > height)
            {
                //landscape
                FlipStack.Orientation = StackOrientation.Horizontal;
                PortraitPicture.IsVisible = false;
                LandscapePicture.IsVisible = true;
            }
            else
            {
                //portrait
                FlipStack.Orientation = StackOrientation.Vertical;
                PortraitPicture.IsVisible = true;
                LandscapePicture.IsVisible = false;
            }
        }

        private void ItemDetails_BindingContextChanged(object sender, EventArgs e)
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
            if (BindingContext is ItemDetailsPageViewModel vm)
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
            if (BindingContext is ItemDetailsPageViewModel vm)
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