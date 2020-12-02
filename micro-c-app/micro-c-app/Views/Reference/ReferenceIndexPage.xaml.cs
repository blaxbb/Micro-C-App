using micro_c_app.Models.Reference;
using micro_c_app.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                Tree.Nodes.Add(new ReferenceTree("A")
                {
                    Nodes = new List<IReferenceItem>()
                    {
                        new ReferenceEntry() {
                            Name = "1",
                            Data = "1111 Page"
                        },
                        new ReferenceEntry() {
                            Name = "2",
                            Data = "22222 Page"
                        },
                        new ReferencePlanData(MicroCLib.Models.Reference.PlanReference.PlanType.Replacement)
                        {
                            Name = "Replacement"
                        }
                    }
                });
                Tree.Nodes.Add(new ReferenceTree("B")
                {
                    Nodes = new List<IReferenceItem>()
                    {
                        new ReferenceEntry() {
                            Name = "Z",
                            Data = "ZZZZZ Page"
                        },
                        new ReferenceEntry() {
                            Name = "Y",
                            Data = "YYYYY Page"
                        },
                    }
                });


                AddPlanItems();




                if(BindingContext is ReferenceIndexPageViewModel vm)
                {
                    foreach(var node in Tree.Nodes)
                    {
                        vm.Nodes.Add(node);
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