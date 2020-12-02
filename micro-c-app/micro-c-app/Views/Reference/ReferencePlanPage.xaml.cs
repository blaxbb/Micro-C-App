using micro_c_app.ViewModels.Reference;
using MicroCLib.Models.Reference;
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
    public partial class ReferencePlanPage : ContentPage
    {
        public ReferencePlanPage()
        {
            InitializeComponent();
            if (BindingContext is ReferencePlanPageViewModel vm)
            {
                vm.PropertyChanged += Vm_PropertyChanged;
            }
            SetupPlans();
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ReferencePlanPageViewModel.Plans))
            {
                SetupPlans();
            }
        }

        private void SetupPlans()
        {
            if (BindingContext is ReferencePlanPageViewModel vm)
            {
                headerGrid.Children.Clear();
                headerGrid.ColumnDefinitions.Clear();
                if (vm.Plans.Count > 0)
                {
                    var tiers = vm.Plans[0].Tiers;
                    for (int i = 0; i < tiers.Count; i++)
                    {
                        var tier = tiers[i];
                        var label = new Label()
                        {
                            Text = $"{tier.Duration} year",
                            Padding = new Thickness(10)
                        };
                        headerGrid.Children.Add(label);
                        Grid.SetColumn(label, i + 1);
                    }
                }


                planGrid.Children.Clear();
                planGrid.ColumnDefinitions.Clear();
                planGrid.RowDefinitions.Clear();
                for (int i = 0; i < vm.Plans.Count; i++)
                {
                    var plan = vm.Plans[i];
                    if(plan == null)
                    {
                        break;
                    }

                    var label = new Label()
                    {
                        Text = $"{plan.MinPrice:$0.00} - {plan.MaxPrice:$0.00}",
                        Padding = new Thickness(10)
                    };
                    planGrid.Children.Add(label);
                    Grid.SetColumn(label, 0);
                    Grid.SetRow(label, i);
                    planGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                    for (int j = 0; j < plan.Tiers.Count; j++)
                    {
                        var tier = plan.Tiers[j];
                        if(tier == null)
                        {
                            break;
                        }

                        var priceLabel = new Label()
                        {
                            Text = tier.Price.ToString("$0.00"),
                            Padding = new Thickness(10)
                        };
                        planGrid.Children.Add(priceLabel);
                        Grid.SetColumn(priceLabel, j + 1);
                        Grid.SetRow(priceLabel, i);
                    }
                }
            }
        }
    }
}