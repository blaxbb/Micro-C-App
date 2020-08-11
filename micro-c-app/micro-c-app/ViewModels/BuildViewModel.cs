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
                var comp = new BuildComponent() { Type = t };
                foreach(var dep in BuildComponentDependency.Dependencies)
                {
                    if(comp.Type == dep.FirstType)
                    {
                        dep.First = comp;
                        comp.Dependencies.Add(dep);
                    }
                    else if(comp.Type == dep.SecondType)
                    {
                        dep.Second = comp;
                        comp.Dependencies.Add(dep);
                    }

                }
                Components.Add(comp);
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

            foreach(var depend in updated?.Component.Dependencies)
            {
                depend.Other(updated.Component)?.OnDependencyStatusChanged();
            }
            updated.Component.OnDependencyStatusChanged();
            OnPropertyChanged(nameof(Components));
            Navigation.PopAsync();
        }
    }
}