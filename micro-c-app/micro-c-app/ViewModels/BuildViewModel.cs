using micro_c_app.Models;
using micro_c_app.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace micro_c_app.ViewModels
{
    public class BuildViewModel : BaseViewModel
    {
        string configID;
        public string ConfigID { get => configID; set { SetProperty(ref configID, value); } }
        public ICommand ComponentSelectClicked { get; }

        public ObservableCollection<BuildComponent> Components { get; }
        public INavigation Navigation { get; internal set; }

        public BuildViewModel()
        {
            Title = "Build";

            MessagingCenter.Subscribe<BuildComponentViewModel>(this, "selected", BuildComponentSelected);

            Components = new ObservableCollection<BuildComponent>();
            foreach (BuildComponent.ComponentType t in Enum.GetValues(typeof(BuildComponent.ComponentType)))
            {
                Components.Add(new BuildComponent() { Type = t });
            }

            ConfigID = "ZZZ";
            ComponentSelectClicked = new Command<BuildComponent>(async (BuildComponent comp) =>
            {
                var componentPage = new BuildComponentPage();
                if (componentPage.BindingContext is BuildComponentViewModel vm)
                {
                    vm.Component = comp;
                }
                await Navigation.PushAsync(componentPage);
            });
        }

        private void BuildComponentSelected(BuildComponentViewModel updated)
        {
            if(updated?.Component.Item == null)
            {
                return;
            }

            StringBuilder b = new StringBuilder();
            foreach(var other in Components.Where(c => c.Item != null))
            {
                if(other == updated.Component)
                {
                    continue;
                }

                foreach(var depend in BuildComponentDependency.Dependencies.Where(d => d.Applicable(updated.Component, other)))
                {
                    var compat = depend.Compatible(updated.Component, other);
                    if (!compat)
                    {
                        b.AppendLine(depend.ErrorText);
                    }
                }
            }
            updated.Component.CompatibilityError = b.ToString();
            OnPropertyChanged(nameof(Components));
            Navigation.PopAsync();
        }
    }
}