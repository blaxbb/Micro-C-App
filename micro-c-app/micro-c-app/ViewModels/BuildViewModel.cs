﻿using micro_c_app.Models;
using micro_c_app.Views;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class BuildViewModel : BaseViewModel
    {
        string configID;
        public string ConfigID { get => configID; set { SetProperty(ref configID, value); } }
        public ICommand ComponentSelectClicked { get; }

        public ObservableCollection<BuildComponent> Components { get; }
        public INavigation Navigation { get; internal set; }

        public float Subtotal => Components.Sum(c => c.Item?.Price ?? 0f);
        public string TaxedTotal => $"({SettingsPage.TaxRate()})% ${(Subtotal * SettingsPage.TaxRateFactor()).ToString("#0.00")}";

        public ICommand SendQuote { get; }

        public BuildViewModel()
        {
            Title = "Build";

            MessagingCenter.Subscribe<BuildComponentViewModel>(this, "selected", BuildComponentSelected);
            MessagingCenter.Subscribe<BuildComponentViewModel>(this, "new",      BuildComponentNew);
            MessagingCenter.Subscribe<BuildComponentViewModel>(this, "removed",  BuildComponentRemove);

            Components = new ObservableCollection<BuildComponent>();
            foreach (BuildComponent.ComponentType t in Enum.GetValues(typeof(BuildComponent.ComponentType)))
            {
                var comp = new BuildComponent() { Type = t };
                comp.AddDependencies(BuildComponentDependency.Dependencies);
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
                componentPage.SetupPlansAndSpecs();
                await Navigation.PushAsync(componentPage);
            });

            SendQuote = new Command(async () => await QuotePageViewModel.DoSendQuote(Components.Where(c => c.Item != null).Select(c => c.Item)));
        }

        private void BuildComponentRemove(BuildComponentViewModel updated)
        {
            var emptyComponents = Components.Where(c => updated.Component.Type == c.Type && c.Item == null).ToList();

            var count = emptyComponents.Count;
            for(int i = 0; i < count - 1; i++)
            {
                Components.Remove(emptyComponents[i]);
            }
            BuildComponentSelected(updated);
        }

        private void BuildComponentNew(BuildComponentViewModel updated)
        {
            var existing = Components.Count(d => d.Type == updated.Component.Type);
            if(existing < BuildComponent.MaxNumberPerType(updated.Component.Type))
            {
                var newItem = new BuildComponent()
                {
                    Type = updated.Component.Type,
                };
                foreach (var d in updated.Component.Dependencies)
                {
                    var clone = d.Clone();
                    newItem.AddDependency(clone);
                    d.Other(updated.Component).AddDependency(clone);
                }
                var index = Components.IndexOf(updated.Component);
                Components.Insert(index + 1, newItem);
            }

            BuildComponentSelected(updated);
        }


        private void BuildComponentSelected(BuildComponentViewModel updated)
        {
            foreach (var depend in updated?.Component.Dependencies)
            {
                depend.Other(updated.Component)?.OnDependencyStatusChanged();
            }


            updated.Component.OnDependencyStatusChanged();
            OnPropertyChanged(nameof(Components));
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(TaxedTotal));
            Navigation.PopAsync();
        }
    }
}