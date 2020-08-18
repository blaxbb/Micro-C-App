using micro_c_app.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuildComponentPage : ContentPage
    {
        public BuildComponentPage()
        {
            InitializeComponent();
        }

        public void SetupPlansAndSpecs()
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
            PlansStackLayout?.Children?.Clear();
            if (BindingContext is BuildComponentViewModel vm)
            {
                if (vm.Component?.Item != null && vm.Component.Type != Models.BuildComponent.ComponentType.Miscellaneous)
                {
                    var plans = Models.Reference.PlanReference.Get(Models.Reference.PlanReference.PlanType.Replacement, vm.Component.Item.Price);
                    foreach(var tier in plans.Tiers)
                    {
                        this.ToolbarItems.Add(new ToolbarItem($"Add {tier.Duration} yr plan", "", () => { vm.BuildComponentAddPlan(tier); }) { Order = ToolbarItemOrder.Secondary });

                        AddSpacer(PlansStackLayout, Color.LightGray);
                        var stack = new StackLayout() { Orientation = StackOrientation.Horizontal };
                        stack.Children.Add(new Label() { Text = $"{tier.Duration} year {plans.Name}", HorizontalOptions = LayoutOptions.StartAndExpand, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Start });
                        stack.Children.Add(new Label() { Text = $"${tier.Price.ToString("#0.00")}", HorizontalOptions = LayoutOptions.End, WidthRequest = 100, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.End });

                        PlansStackLayout.Children.Add(stack);
                    }
                }
            }
        }

        private void SetSpecItems()
        {
            SpecsStackLayout.Children.Clear();
            if (BindingContext is BuildComponentViewModel vm)
            {
                if (vm?.Component?.Item?.Specs != null)
                {
                    foreach (var spec in vm.Component.Item.Specs)
                    {
                        AddSpacer(SpecsStackLayout, Color.LightGray);
                        var stack = new StackLayout() { Orientation = StackOrientation.Horizontal };
                        stack.Children.Add(new Label()
                        {
                            Text = spec.Key,
                            HorizontalOptions = LayoutOptions.Start,
                            VerticalTextAlignment = TextAlignment.Center,
                            HorizontalTextAlignment = TextAlignment.Start,
                            MinimumWidthRequest = 200,
                            WidthRequest = 200
                        });
                        stack.Children.Add(new Label()
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

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > height)
            {
                Grid.SetColumn(ButtonContainer, 1);
                Grid.SetRow(ButtonContainer, 0);
            }
            else
            {
                Grid.SetColumn(ButtonContainer, 0);
                Grid.SetRow(ButtonContainer, 1);
            }
        }
    }
}