using micro_c_app.Models;
using micro_c_app.Models.Reference;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class BuildComponentViewModel : BaseViewModel
    {
        private BuildComponent? component;

        public BuildComponent? Component { get => component; set { SetProperty(ref component, value); OnPropertyChanged(nameof(ItemExists)); OnPropertyChanged(nameof(ItemNotExists)); } }
        public ICommand ProductFound { get; }
        public ICommand SearchError { get; }
        public ICommand Remove { get; }
        public bool ItemExists => Component?.Item != null;
        public bool ItemNotExists => !ItemExists;

        protected override Dictionary<string, ICommand> Actions
        {
            get
            {
                var res = new Dictionary<string, ICommand>();
                if(Component == null)
                {
                    return res;
                }

                PlanReference? plans = null;
                if (Component.Type == Models.BuildComponent.ComponentType.BuildService)
                {
                    plans = Models.Reference.PlanReference.Get(PlanReference.PlanType.Build_Plan, BuildPageViewModel.CurrentSubTotal);
                }
                else
                {
                    if (Component?.Item != null)
                    {
                        plans = PlanReference.Get(PlanReference.PlanType.Replacement, Component.Item.Price);
                    }
                }

                if (plans != null)
                {
                    foreach (var tier in plans.Tiers)
                    {
                        res.Add($"Add {tier.Duration}yr plan", new Command(() => { BuildComponentAddPlan(tier); }));
                    }
                }

                return res;
            }
        }
        public BuildComponentViewModel()
        {
            Title = "Details";

            ProductFound = new Command<Item>((item) =>
            {
                if (Component != null)
                {
                    var isNew = Component.Item == null;
                    Component.Item = item;

                    if (isNew)
                    {
                        MessagingCenter.Send(this, "new");
                    }
                    else
                    {
                        MessagingCenter.Send(this, "selected");
                    }
                }
            });

            SearchError = new Command<string>(async (message) =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Error", message, "Ok");
                });
            });

            Remove = new Command(() =>
            {
                if (Component != null)
                {
                    Component.Item = null;
                    MessagingCenter.Send(this, "removed");
                }
            });
        }

        public void BuildComponentAddPlan(PlanTier tier)
        {
            MessagingCenter.Send(this, "add_plan", tier);
        }
    }
}