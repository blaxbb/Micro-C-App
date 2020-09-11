using micro_c_app.Models;
using micro_c_app.Models.Reference;
using micro_c_app.Views;
using micro_c_app.Views.CollectionFile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class BuildPageViewModel : BaseViewModel
    {
        private string? buildURL;
        public ICommand ComponentSelectClicked { get; }

        public ObservableCollection<BuildComponent> Components { get; }

        public float Subtotal => Components.Sum(c => c.Item?.Price ?? 0f);
        public string TaxedTotal => $"({SettingsPage.TaxRate()})% ${(Subtotal * SettingsPage.TaxRateFactor()).ToString("#0.00")}";

        public static float CurrentSubTotal { get; set; }

        public ICommand SendQuote { get; }
        public ICommand Reset { get; }
        public ICommand Save { get; }
        public ICommand Load { get; }
        public ICommand Export { get; }
        public ICommand OpenURL { get; }

        public string? BuildURL { get => buildURL; set => SetProperty(ref buildURL, value); }

        protected override Dictionary<string, ICommand> Actions => new Dictionary<string, ICommand>()
        {
            {"Send", SendQuote },
            {"Reset", Reset },
            {"Save", Save },
            {"Load", Load },
            {"Export", Export }
        };

        public static event Action? CellUpdated;

        public BuildPageViewModel()
        {
            Title = "Build";

            MessagingCenter.Subscribe<BuildComponentViewModel>(this, "selected", BuildComponentSelected);
            MessagingCenter.Subscribe<BuildComponentViewModel>(this, "new", BuildComponentNew);
            MessagingCenter.Subscribe<BuildComponentViewModel>(this, "removed", BuildComponentRemove);
            MessagingCenter.Subscribe<BuildComponentViewModel, PlanTier>(this, "add_plan", BuildComponentAddPlan);

            MessagingCenter.Subscribe<SettingsPageViewModel>(this, SettingsPageViewModel.SETTINGS_UPDATED_MESSAGE, (_) => { UpdateProperties(); });

            if (RestoreState.Instance.BuildComponents == null)
            {
                Components = new ObservableCollection<BuildComponent>();
                SetupDefaultComponents();
            }
            else
            {
                Components = new ObservableCollection<BuildComponent>(RestoreState.Instance.BuildComponents);
                foreach (var comp in Components)
                {
                    comp.AddDependencies(FieldContainsDependency.Dependencies);
                }
            }

            ComponentSelectClicked = new Command<BuildComponent>(async (BuildComponent comp) =>
            {
                var componentPage = new BuildComponentPage();
                if (componentPage.BindingContext is BuildComponentViewModel vm)
                {
                    vm.Component = comp;
                }
                componentPage.Setup();
                await Shell.Current.Navigation.PushAsync(componentPage);
            });

            SendQuote = new Command(async () => await QuotePageViewModel.DoSendQuote(Components.Where(c => c.Item != null).Select(c => c.Item!)));

            Reset = new Command(async () =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var reset = await Shell.Current.DisplayAlert("Reset", "Are you sure you want to reset the build?", "Yes", "No");
                    if (reset)
                    {
                        BuildURL = null;
                        Components.Clear();
                        SetupDefaultComponents();
                        UpdateProperties();
                        SaveRestore();
                    }
                });
            });

            Save = new Command(async () =>
            {
                var vm = new CollectionSavePageViewModel("build", Components.ToList());

                await Shell.Current.Navigation.PushModalAsync(new CollectionSavePage() { BindingContext = vm });
            });

            Load = new Command(async () =>
            {
                var vm = new CollectionLoadPageViewModel<BuildComponent>("build");
                MessagingCenter.Subscribe<CollectionLoadPageViewModel<BuildComponent>>(this, "load", DoLoad, vm);
                await Shell.Current.Navigation.PushModalAsync(new CollectionLoadPage() { BindingContext = vm });
            });

            Export = new Command(async () =>
            {
                var vm = new ProgressPageViewModel()
                {
                    Description = "Exporting build to MCOL",
                    TotalItems = Components.Count(c => c.Item != null && c.Item.ID != null)
                };
                var page = new ProgressPage() { BindingContext = vm };

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.Navigation.PushModalAsync(page, true);
                });

                try
                {
                    using (var client = new HttpClient())
                    {
                        List<string> hitCategories = new List<string>();
                        int cnt = 0;
                        for (int i = 0; i < Components.Count; i++)
                        {
                            var comp = Components[i];
                            if (comp.Item == null || comp.Item.ID == null)
                            {
                                continue;
                            }
                            cnt++;
                            await Device.InvokeOnMainThreadAsync(() =>
                            {
                                vm.CurrentItem = cnt;
                            });
                            var selectorID = BuildComponent.MCOLSelectorIDForType(comp.Type);
                            bool duplicateSelector = hitCategories.Contains(selectorID);
                            var url = $"https://www.microcenter.com/site/content/custom-pc-builder.aspx?toselectorId={selectorID}&configuratorId=1&productId={comp.Item.ID}&productName={comp.Item.Name}&newItem={(duplicateSelector ? "true" : "false")}";
                            if (!duplicateSelector)
                            {
                                hitCategories.Add(selectorID);
                            }

                            var result = await client.GetAsync(url);
                            var body = await result.Content.ReadAsStringAsync();
                            var match = Regex.Match(body, "value=\"(.*?)\" name=\"shareURL\" id=\"shareURL\">");
                            if (match.Success)
                            {
                                BuildURL = match.Groups[1].Value;
                            }
                        }
                    }
                }
                catch(Exception e)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.DisplayAlert("Exception", e.ToString(), "Ok");
                    });
                }

                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.Navigation.PopModalAsync();
                });
            });

            Components.CollectionChanged += (sender, args) => { SaveRestore(); };

            OpenURL = new Command(async () =>
            {
                if (!string.IsNullOrWhiteSpace(BuildURL))
                {
                    await Launcher.TryOpenAsync(BuildURL);
                }
            });
        }

        private void DoLoad(CollectionLoadPageViewModel<BuildComponent> obj)
        {
            Components.Clear();
            foreach (var i in obj.Result)
            {
                Components.Add(i);
            }

            foreach (var comp in Components)
            {
                comp.AddDependencies(FieldContainsDependency.Dependencies);
            }
        }

        void SetupDefaultComponents()
        {
            foreach (BuildComponent.ComponentType t in Enum.GetValues(typeof(BuildComponent.ComponentType)))
            {
                if (BuildComponent.MaxNumberPerType(t) > 0)
                {
                    var comp = new BuildComponent() { Type = t };
                    comp.AddDependencies(FieldContainsDependency.Dependencies);
                    Components.Add(comp);
                }
            }
        }

        private void SaveRestore()
        {
            RestoreState.Instance.BuildComponents = Components.ToList();
            RestoreState.Save();
        }

        private void BuildComponentRemove(BuildComponentViewModel updated)
        {
            if(updated.Component == null)
            {
                return;
            }

            var emptyComponents = Components.Where(c => updated.Component.Type == c.Type && c.Item == null).ToList();

            var count = emptyComponents.Count;
            ///
            // for types that we don't want the user to add manually, remove all empty items, othewise leave one for interaction
            //
            var stop = BuildComponent.MaxNumberPerType(updated.Component.Type) == 0 ? count : count - 1;
            for (int i = 0; i < stop; i++)
            {
                Components.Remove(emptyComponents[i]);
            }
            BuildComponentSelected(updated);
        }

        private void BuildComponentNew(BuildComponentViewModel updated)
        {
            if (updated.Component == null)
            {
                return;
            }

            var existing = Components.Count(d => d.Type == updated.Component.Type);
            if (existing < BuildComponent.MaxNumberPerType(updated.Component.Type))
            {
                var newItem = new BuildComponent()
                {
                    Type = updated.Component.Type,
                };
                foreach (var d in updated.Component.Dependencies)
                {
                    var clone = d.Clone();
                    newItem.AddDependency(clone);
                    d.Other(updated.Component)?.AddDependency(clone);
                }
                var index = Components.IndexOf(updated.Component);
                Components.Insert(index + 1, newItem);
            }

            BuildComponentSelected(updated);
        }


        private void BuildComponentSelected(BuildComponentViewModel updated)
        {
            if(updated?.Component?.Dependencies == null)
            {
                return;
            }

            foreach (var depend in updated.Component.Dependencies)
            {
                depend.Other(updated.Component)?.OnDependencyStatusChanged();
            }

            CurrentSubTotal = Subtotal;
            updated.Component.OnDependencyStatusChanged();
            UpdateProperties();
            Shell.Current.Navigation.PopAsync();
            CellUpdated?.Invoke();
            SaveRestore();
        }

        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(Components));
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(TaxedTotal));
        }

        public void BuildComponentAddPlan(BuildComponentViewModel vm, PlanTier tier)
        {
            if (vm == null || tier == null)
            {
                return;
            }

            Components.Add(new BuildComponent() { Type = BuildComponent.ComponentType.Plan, Item = new Item() { Name = $"{tier.Duration} year protection on {vm.Component?.Item?.Name}", Price = tier.Price } });
            BuildComponentSelected(vm);
        }
    }
}