using micro_c_app.Models;
using micro_c_app.Views;
using micro_c_app.Views.CollectionFile;
using MicroCLib.Models;
using MicroCLib.Models.Reference;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using static MicroCLib.Models.BuildComponent;

namespace micro_c_app.ViewModels
{
    public class BuildPageViewModel : BaseViewModel
    {
        private string? buildURL;
        public ICommand ComponentSelectClicked { get; }

        public ObservableCollection<BuildComponent> Components { get; }

        public float Subtotal => Components.Sum(c => c.Item?.Price ?? 0f);
        public string TaxedTotal => $"({SettingsPage.TaxRate()})% ${Subtotal * SettingsPage.TaxRateFactor():#0.00}";

        public static float CurrentSubTotal { get; set; }
        public ICommand Reset { get; }
        public ICommand Save { get; }
        public ICommand Load { get; }
        public ICommand OpenURL { get; }
        public ICommand BatchScan { get; }

        public string? BuildURL { get => buildURL; set => SetProperty(ref buildURL, value); }

        protected override Dictionary<string, ICommand> Actions => new Dictionary<string, ICommand>()
        {
            {"Reset", Reset },
            {"Save", Save },
            {"Load", Load },
            { "Batch", BatchScan }
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
            }
            UpdateProperties();
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

            Components.CollectionChanged += (sender, args) => { SaveRestore(); };

            OpenURL = new Command(async () =>
            {
                if (!string.IsNullOrWhiteSpace(BuildURL))
                {
                    await Launcher.TryOpenAsync(BuildURL);
                }
            });

            BatchScan = new Command(() => DoBatchScan());
        }

        private void DoLoad(CollectionLoadPageViewModel<BuildComponent> obj)
        {
            Components.Clear();
            foreach (var i in obj.Result)
            {
                Components.Add(i);
            }
        }

        void SetupDefaultComponents()
        {
            foreach (BuildComponent.ComponentType t in PresetBYO())
            {
                if (BuildComponent.MaxNumberPerType(t) > 0)
                {
                    var comp = new BuildComponent() { Type = t };
                    Components.Add(comp);
                }
            }
        }

        private static IEnumerable<ComponentType> PresetBYO()
        {
            yield return ComponentType.BuildService;
            yield return ComponentType.OperatingSystem;
            yield return ComponentType.CPU;
            yield return ComponentType.Motherboard;
            yield return ComponentType.GPU;
            yield return ComponentType.RAM;
            yield return ComponentType.PowerSupply;
            yield return ComponentType.SSD;
            yield return ComponentType.HDD;
            yield return ComponentType.Case;
            yield return ComponentType.CPUCooler;
            yield return ComponentType.WaterCoolingKit;
            yield return ComponentType.CaseFan;
            yield return ComponentType.Miscellaneous;
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
                var index = Components.IndexOf(updated.Component);
                Components.Insert(index + 1, newItem);
            }

            BuildComponentSelected(updated);
        }


        private void BuildComponentSelected(BuildComponentViewModel updated)
        {
            //CurrentSubTotal = Subtotal;
            Shell.Current.Navigation.PopAsync();
            UpdateProperties();
        }

        private void AddNewItem(Item item, ComponentType type)
        {
            var comp = new BuildComponent()
            {
                Type = type,
                Item = item
            };
            Components.Add(comp);
            UpdateProperties();
        }

        private void DoBatchScan()
        {
            SearchView.DoScan(Shell.Current.Navigation, async (result, progress) => {
                System.Diagnostics.Debug.WriteLine(result);

                try
                {
                    int queryAttempts = 0;
                    //go back here on error, so that we can retry the request a few times
                    const int NUM_RETRY_ATTEMPTS = 5;

                    var storeId = SettingsPage.StoreID();

                startQuery:
                    queryAttempts++;

                    try
                    {
                        var results = await Search.LoadQuery(result, storeId, null, Search.OrderByMode.match, 1, progress: progress);
                        if (results.Items.Count == 1)
                        {
                            var stub = results.Items.First();
                            var item = await Item.FromUrl(stub.URL, storeId, progress: progress);
                            if (item != null)
                            {
                                //
                                // Check if there is an open slot to put the item in
                                //
                                for(int i = item.Categories.Count - 1; i >= 0; i--)
                                {
                                    var cat = item.Categories[i];
                                    var comp = Components.FirstOrDefault(c => c.Item == null && c.CategoryFilter == cat.Filter);
                                    if(comp != null)
                                    {
                                        comp.Item = item;
                                        UpdateProperties();
                                        return;
                                    }
                                }

                                //
                                // If there is no open component, add one
                                //
                                for (int i = item.Categories.Count - 1; i >= 0; i--)
                                {
                                    var cat = BuildComponent.TypeForCategoryFilter(item.Categories[i].Filter);
                                    if(cat != ComponentType.None)
                                    {
                                        AddNewItem(item, cat);
                                        return;
                                    }
                                }
                                AddNewItem(item, ComponentType.Miscellaneous);
                                return;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (queryAttempts > NUM_RETRY_ATTEMPTS)
                        {
                            await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
                            return;
                        }
                    }
                    if (queryAttempts > NUM_RETRY_ATTEMPTS)
                    {
                        await Shell.Current.DisplayAlert("Error", $"Failed to find item \"{result}\"", "Ok");
                    }
                    else
                    {
                        progress?.Report(new ProgressInfo($"Retrying query...{queryAttempts}/{NUM_RETRY_ATTEMPTS}", 0));
                        await Task.Delay(1000);
                        goto startQuery;
                    }
                }
                catch (Exception e)
                {
                    await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
                }
            }, batchMode: true);
        }

        private void UpdateProperties()
        {
            foreach(var comp in Components)
            {
                comp.ErrorText = "";
                comp.HintText = "";
            }
            foreach(var dep in BuildComponentDependency.Dependencies)
            {
                var items = Components.Where(comp => comp.Item != null).Select(comp => comp.Item).ToList();
                var errors = dep.HasErrors(items);
                foreach(var item in errors)
                {
                    var matchingComp = Components.FirstOrDefault(comp => comp.Item == item.Primary);
                    matchingComp.ErrorText += item.Text.Replace("\n", ", ") + "\n\n";
                }

                foreach (var comp in Components)
                {
                    if(comp.Item != null)
                    {
                        continue;
                    }

                    var hint = dep.HintText(items, comp.Type);
                    if (!string.IsNullOrWhiteSpace(hint))
                    {
                        comp.HintText += hint.Replace("\n", ", ") + "\n\n";
                    }
                }
            }



            CurrentSubTotal = Subtotal;
            CellUpdated?.Invoke();
            SaveRestore();

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