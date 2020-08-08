using micro_c_app.Models;
using micro_c_app.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            Components = new ObservableCollection<BuildComponent>()
            {
                new BuildComponent(){Type = BuildComponent.ComponentType.CPU},
                new BuildComponent(){Type = BuildComponent.ComponentType.Motherboard},
                new BuildComponent(){Type = BuildComponent.ComponentType.RAM}
            };

            ConfigID = "ZZZ";
            ComponentSelectClicked = new Command<BuildComponent>(async (BuildComponent comp) =>
            {
                var componentPage = new BuildComponentPage();
                if (componentPage.BindingContext is BuildComponentViewModel vm)
                {
                    vm.Component = comp;
                }

                await Navigation.PushModalAsync(componentPage);
            });
        }

        private void BuildComponentSelected(BuildComponentViewModel obj)
        {
            OnPropertyChanged(nameof(Components));
            Navigation.PopModalAsync();
        }
    }
}