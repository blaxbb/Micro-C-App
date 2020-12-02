using micro_c_app.Models.Reference;
using micro_c_app.ViewModels.Reference;
using micro_c_app.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class ReferenceIndexPageViewModel : BaseViewModel
    {
        private List<IReferenceItem> nodes;
        public List<IReferenceItem> Nodes { get => nodes; set => SetProperty(ref nodes, value); }
        public ICommand SelectedCommand { get; }

        public ReferenceIndexPageViewModel()
        {
            Nodes = new List<IReferenceItem>();

            SelectedCommand = new Command<IReferenceItem>(async (node) =>
            {
                Debug.WriteLine(node.Name);
                if (node is ReferenceTree tree)
                {
                    if (tree.Nodes != null && tree.Nodes.Count > 0)
                    {
                        await Device.InvokeOnMainThreadAsync(async () =>
                        {
                            var page = new ReferenceIndexPage()
                            {
                                Title = tree.Name,
                            };
                            if (page.BindingContext is ReferenceIndexPageViewModel vm)
                            {
                                vm.Nodes = tree.Nodes;
                            }
                            await Shell.Current.Navigation.PushAsync(page);
                        });
                    }
                }
                else if(node is ReferenceEntry entry)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.DisplayAlert("Alert", entry.Data, "Ok");
                    });
                }
                else if(node is ReferencePlanData plans)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var page = new ReferencePlanPage()
                        {
                            Title = node.Name,
                        };
                        if (page.BindingContext is ReferencePlanPageViewModel vm)
                        {
                            vm.Plans = plans.Plans;
                        }
                        await Shell.Current.Navigation.PushAsync(page);
                    });
                }
            });
        }
    }
}
