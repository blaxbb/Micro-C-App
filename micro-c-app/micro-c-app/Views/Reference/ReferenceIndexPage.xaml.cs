using micro_c_app.Models.Reference;
using micro_c_app.ViewModels;
using micro_c_app.ViewModels.Reference;
using micro_c_app.Views.Reference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MicroCLib.Models.Reference.PlanReference;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReferenceIndexPage : ContentPage
    {
        static bool Initialized = false;
        public static ReferenceTree Tree { get; private set; }
        public ReferenceIndexPage()
        {
            InitializeComponent();
            if(!Initialized)
            {
                Initialized = true;
                Tree = new ReferenceTree("Root");

                AddPlanItems();
                AddPageItems();

                if(BindingContext is ReferenceIndexPageViewModel vm)
                {
                    foreach(var node in Tree.Nodes)
                    {
                        vm.Nodes.Add(node);
                    }
                }
            }
        }

        public static async void NavigateTo(string path)
        {
            var parts = path.Split('/').Skip(1);
            var node = Tree.GetNode(parts);
            await NavigateTo(node);
        }

        public static async Task NavigateTo(IReferenceItem node)
        {
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
            else if (node is ReferenceEntry entry)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var page = new ReferenceWebViewPage()
                    {
                        Title = node.Name,
                    };
                    if (page.BindingContext is ReferenceWebViewPageViewModel vm)
                    {
                        vm.Text = entry.Data;
                    }
                    await Shell.Current.Navigation.PushAsync(page);
                });
            }
            else if (node is ReferencePlanData plans)
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
        }

        private void AddPageItems()
        {
            if (BindingContext is ReferenceIndexPageViewModel vm)
            {
                var assembly = IntrospectionExtensions.GetTypeInfo(typeof(App)).Assembly;
                foreach (var res in assembly.GetManifestResourceNames())
                {
                    System.Diagnostics.Debug.WriteLine("found resource: " + res);
                    var match = Regex.Match(res, "micro_c_app\\.Assets\\.Pages\\.(.*?)\\.md");
                    //var match = Regex.Match(res, "micro_c_app\\.Assets\\.Pages\\.(.*?)\\.(?:md|dev)");
                    if (match.Success)
                    {
                        var name = match.Groups[1].Value;
                        var stream = assembly.GetManifestResourceStream(res);
                        using var reader = new StreamReader(stream);
                        var text = reader.ReadToEnd();

                        var path = name.Split('.');
                        var parent = Tree.CreateRoute(path);
                        parent.Nodes.Add(new ReferenceEntry()
                        {
                            Name = path.Last(),
                            Data = text
                        });
                    }
                }
            }
        }

        private void AddPlanItems()
        {
            var planRoot = new ReferenceTree()
            {
                Name = "Plans"
            };

            Tree.Nodes.Add(planRoot);

            planRoot.Nodes.Add(new ReferencePlanData(PlanType.Replacement));
            planRoot.Nodes.Add(new ReferencePlanData(PlanType.Small_Electronic_ADH));
            planRoot.Nodes.Add(new ReferencePlanData(PlanType.Carry_In));
            planRoot.Nodes.Add(new ReferencePlanData(PlanType.Build_Plan));
            planRoot.Nodes.Add(new ReferencePlanData(PlanType.BYO_Replacement));

            AddApplePlans(planRoot);
            AddDesktopPlans(planRoot);
            AddLaptopPlans(planRoot);
            AddTabletPlans(planRoot);
        }

        private void AddTabletPlans(ReferenceTree planRoot)
        {
            var root = new ReferenceTree("Tablets");
            planRoot.Nodes.Add(root);

            root.Nodes.Add(new ReferencePlanData(PlanType.Tablet_ADH));
            root.Nodes.Add(new ReferencePlanData(PlanType.Tablet_DOP));
            root.Nodes.Add(new ReferencePlanData(PlanType.Tablet_Extension));
        }

        private void AddLaptopPlans(ReferenceTree planRoot)
        {
            var root = new ReferenceTree("Laptops");
            planRoot.Nodes.Add(root);

            root.Nodes.Add(new ReferencePlanData(PlanType.Laptop_ADH));
            root.Nodes.Add(new ReferencePlanData(PlanType.Laptop_DOP));
            root.Nodes.Add(new ReferencePlanData(PlanType.Laptop_Extension));
        }

        private void AddDesktopPlans(ReferenceTree planRoot)
        {
            var root = new ReferenceTree("Desktops");
            planRoot.Nodes.Add(root);

            root.Nodes.Add(new ReferencePlanData(PlanType.Desktop_ADH));
            root.Nodes.Add(new ReferencePlanData(PlanType.Desktop_DOP));
            root.Nodes.Add(new ReferencePlanData(PlanType.Desktop_Extension));
        }

        private void AddApplePlans(ReferenceTree planRoot)
        {
            var appleRoot = new ReferenceTree("Apple");
            planRoot.Nodes.Add(appleRoot);

            appleRoot.Nodes.Add(new ReferenceTree() {
                Name = "13 inch Mac Books",
                Nodes = new List<IReferenceItem>()
                {
                    new ReferencePlanData(PlanType.Apple_Plans_ADH_13),
                    new ReferencePlanData(PlanType.Apple_Plans_13),
                    new ReferencePlanData(PlanType.AppleCare_13_MBA),
                    new ReferencePlanData(PlanType.AppleCare_13_MBP),
                }
            });
            appleRoot.Nodes.Add(new ReferenceTree()
            {
                Name = "15-16 inch Mac Book Pro",
                Nodes = new List<IReferenceItem>()
                {
                    new ReferencePlanData(PlanType.Apple_Plans_ADH_15_and_16),
                    new ReferencePlanData(PlanType.Apple_Plans_15_and_16),
                    new ReferencePlanData(PlanType.AppleCare_15),
                }
            });
            appleRoot.Nodes.Add(new ReferenceTree()
            {
                Name = "Mac Mini",
                Nodes = new List<IReferenceItem>()
                {
                    new ReferencePlanData(PlanType.Apple_Plans_ADH_Mac_Mini),
                    new ReferencePlanData(PlanType.Apple_Plans_Mac_Mini),
                    new ReferencePlanData(PlanType.AppleCare_Mac_Mini),
                }
            });
            appleRoot.Nodes.Add(new ReferenceTree()
            {
                Name = "Mac Pro",
                Nodes = new List<IReferenceItem>()
                {
                    new ReferencePlanData(PlanType.Apple_Plans_ADH_Mac_Pro),
                    new ReferencePlanData(PlanType.Apple_Plans_Mac_Pro),
                    new ReferencePlanData(PlanType.AppleCare_Mac_Pro),
                }
            });
            appleRoot.Nodes.Add(new ReferenceTree()
            {
                Name = "iMac",
                Nodes = new List<IReferenceItem>()
                {
                    new ReferencePlanData(PlanType.Apple_Plans_ADH_iMac),
                    new ReferencePlanData(PlanType.Apple_Plans_iMac),
                    new ReferencePlanData(PlanType.AppleCare_iMac),
                }
            });
            appleRoot.Nodes.Add(new ReferenceTree()
            {
                Name = "iPad",
                Nodes = new List<IReferenceItem>()
                {
                    new ReferencePlanData(PlanType.Apple_Plans_ADH_iPad),
                    new ReferencePlanData(PlanType.Apple_Plans_iPad),
                    new ReferencePlanData(PlanType.AppleCare_iPad),
                    new ReferencePlanData(PlanType.AppleCare_iPad_Pro),
                }
            });
            appleRoot.Nodes.Add(new ReferenceTree()
            {
                Name = "Watch",
                Nodes = new List<IReferenceItem>()
                {
                    new ReferencePlanData(PlanType.AppleCare_Watch_S3),
                    new ReferencePlanData(PlanType.AppleCare_Watch_S4_S5),
                    new ReferencePlanData(PlanType.AppleCare_Watch_Stainless),
                }
            });
            appleRoot.Nodes.Add(new ReferenceTree()
            {
                Name = "Apple Care Misc",
                Nodes = new List<IReferenceItem>()
                {
                    new ReferencePlanData(PlanType.AppleCare_Apple_TV),
                    new ReferencePlanData(PlanType.AppleCare_Display),
                    new ReferencePlanData(PlanType.AppleCare_iPod_Touch),
                    new ReferencePlanData(PlanType.AppleCare_iPhone),
                    new ReferencePlanData(PlanType.AppleCare_HomePod),
                    new ReferencePlanData(PlanType.AppleCare_Headphones),
                }
            });
        }
    }
}