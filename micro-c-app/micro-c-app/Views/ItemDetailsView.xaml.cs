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
            StripeStacks();
        }

        private static void ItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ItemDetailsView view && view.BindingContext is ItemDetailsViewViewModel vm)
            {
                vm.Item = newValue as Item;
                vm.FastView = view.FastView;
                view.StripeStacks();
            }
        }

        private void StripeStacks()
        {
            var stripeColor = Application.Current.UserAppTheme == OSAppTheme.Dark ||
                              (Application.Current.UserAppTheme == OSAppTheme.Unspecified && Application.Current.RequestedTheme == OSAppTheme.Dark)
                              ? Color.FromHex("FF595959") : Color.LightGray;

            var stacks = new []{ SpecsStack, PlanStack, ClearanceStack };
            foreach(var stack in stacks)
            {
                for(int i = 0; i < stack.Children.Count; i++)
                {
                    var child = stack.Children[i];
                    if(i % 2 == 0)
                    {
                        child.BackgroundColor = stripeColor;
                    }
                }
            }
        }
    }
}