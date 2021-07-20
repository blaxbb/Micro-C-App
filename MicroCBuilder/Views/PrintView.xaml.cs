using MicroCLib.Models;
using MicroCLib.Models.Reference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MicroCBuilder.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PrintView : UserControl
    {
        public PrintView()
        {
            this.InitializeComponent();
        }

        public static BuildComponent? GetPlan(int duration, BuildComponent Component)
        {
            switch (Component.Type)
            {
                case BuildComponent.ComponentType.OperatingSystem:
                case BuildComponent.ComponentType.BuildService:
                    return null;
            }

            var price = Component.Item.Price;
            var type = price >= 500 ? PlanReference.PlanType.Carry_In : PlanReference.PlanType.Replacement;
            var plan = PlanReference.Get(type, price);
            if (plan == null)
            {
                return null;
            }

            var tier = plan.Tiers.FirstOrDefault(p => p.Duration == duration);
            var comp = new BuildComponent()
            {
                Type = BuildComponent.ComponentType.Plan,
                Item = new Item()
                {
                    Name = $"{duration} Year Plan",
                    Price = tier.Price,
                    OriginalPrice = tier.Price,
                    Brand = "Micro Center",
                    Quantity = 1,
                },
            };

            return comp;
        }
    }
}
