using micro_c_app.Models.Reference;
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
            this.SetupActionButton();
        }

        public void Setup()
        {
            SetPlanItems();
            SetSpecItems();
            if(BindingContext is BuildComponentViewModel vm)
            {
                if(vm.Component.AutoSearch() && vm.Component.Item == null)
                {
                    SearchView.OrderBy = SearchView.OrderByMode.pricelow;
                    _ = SearchView.OnSubmit("");
                }
            }
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
                if (vm.Component?.Item != null && vm.Component.PlanApplicable())
                {
                    PlanReference plans;
                    if (vm.Component.Type == Models.BuildComponent.ComponentType.BuildService)
                    {
                        plans = Models.Reference.PlanReference.Get(PlanReference.PlanType.Build_Plan, BuildPageViewModel.CurrentSubTotal);
                    }
                    else
                    {
                        plans = Models.Reference.PlanReference.Get(Models.Reference.PlanReference.PlanType.Replacement, vm.Component.Item.Price);
                    }

                    foreach(var tier in plans.Tiers)
                    {
                        //iOS is handled in BuildComponentViewModel.cs Actions
                        if (Device.RuntimePlatform == "Android")
                        {
                            this.ToolbarItems.Add(new ToolbarItem($"Add {tier.Duration} yr plan", "", () => { vm.BuildComponentAddPlan(tier); }) { Order = ToolbarItemOrder.Secondary });
                        }

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
                //FlipStack.Orientation = StackOrientation.Horizontal;
                if(grid.RowDefinitions.Count > 1 && grid.ColumnDefinitions.Count > 0)
                {
                    grid.RowDefinitions[1].Height = 0;
                    grid.ColumnDefinitions[1].Width = GridLength.Star;
                }

                grid.RowSpacing = 0;
                grid.ColumnSpacing = 20;

                Grid.SetRow(ItemInfo, 0);
                Grid.SetColumn(ItemInfo, 1);
                //SearchView.Orientation = "Vertical";
            }
            else
            {
                //FlipStack.Orientation = StackOrientation.Vertical;
                if (grid.RowDefinitions.Count > 1 && grid.ColumnDefinitions.Count > 0)
                {
                    grid.RowDefinitions[1].Height = new GridLength(2.25, GridUnitType.Star);
                    grid.ColumnDefinitions[1].Width = 0;
                }
                grid.RowSpacing = 20;
                grid.ColumnSpacing = 0;
                Grid.SetRow(ItemInfo, 1);
                Grid.SetColumn(ItemInfo, 0);
                //SearchView.Orientation = "Horizontal";
            }
        }
    }
}