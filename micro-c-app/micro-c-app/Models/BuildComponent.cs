using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace micro_c_app.Models
{
    public class BuildComponent : NotifyPropertyChangedItem
    {
        private Item item;

        public Item Item { get => item; set { SetProperty(ref item, value); } }

        public enum ComponentType
        {
            CPU,
            Motherboard,
            RAM,
            PowerSupply,
            GPU,
            SSD,
            HDD,
            CPUCooler,
            CaseFan
        }
        public ComponentType Type { get; set; }
        public string CategoryFilter => CategoryFilterForType(Type);

        public string ComponentLabel => $"{Type.ToString()} - {Item?.Name}";

        public static string CategoryFilterForType(ComponentType type)
        {
            //from microcenter.com search N field ex: &N=4294966995

            switch (type)
            {
                case ComponentType.CPU:
                    return "4294966995";
                case ComponentType.Motherboard:
                    return "4294966996";
                case ComponentType.RAM:
                    return "4294966653";
                case ComponentType.PowerSupply:
                    return "4294966654";
                case ComponentType.GPU:
                    return "4294966938";
                case ComponentType.SSD:
                    return "4294945779";
                case ComponentType.HDD:
                    return "4294945772";
                case ComponentType.CPUCooler:
                    return "4294966927";
                case ComponentType.CaseFan:
                    return "4294966926";
                default:
                    return "";
            }
        }
    }
}
