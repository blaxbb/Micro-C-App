using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace MicroCLib.Models
{
    public class BuildComponent : NotifyPropertyChangedItem
    {
        private Item? item;
        public Item? Item { get => item; set { SetProperty(ref item, value); OnPropertyChanged(nameof(ComponentLabel)); } }

        [JsonIgnore]
        public List<BuildComponentDependency> Dependencies { get; }

        public enum ComponentType
        {
            BuildService,
            CPU,
            Motherboard,
            RAM,
            Case,
            PowerSupply,
            GPU,
            SSD,
            HDD,
            CPUCooler,
            CaseFan,
            OperatingSystem,
            Miscellaneous,
            Plan
        }
        public ComponentType Type { get; set; }
        [JsonIgnore]
        public string CategoryFilter => CategoryFilterForType(Type);
        [JsonIgnore]
        public string ComponentLabel => Item == null ? Type.ToString() : $"{Item.Name}";
        [JsonIgnore]
        public string ErrorText => String.Join("\n", Dependencies.Where(d => !d.Compatible()).Select(d => d.ErrorText));
        [JsonIgnore]
        public string HintText => Item == null ? String.Join("\n", Dependencies.Where(d => d.Other(this)?.item != null).Select(d => d.HintText())) : "";

        public BuildComponent()
        {
            Dependencies = new List<BuildComponentDependency>();
        }

        public void OnDependencyStatusChanged()
        {
            OnPropertyChanged(nameof(ErrorText));
            OnPropertyChanged(nameof(HintText));
        }

        public void AddDependencies(List<BuildComponentDependency> dependencies)
        {
            foreach (var dep in dependencies)
            {
                AddDependency(dep);

            }
        }

        public void AddDependency(BuildComponentDependency dependency)
        {
            if(dependency.SetRelevant(this))
            {
                Dependencies.Add(dependency);
            }

            
        }

        public bool PlanApplicable()
        {
            switch (Type)
            {
                case ComponentType.CPU:
                case ComponentType.Motherboard:
                case ComponentType.RAM:
                case ComponentType.Case:
                case ComponentType.PowerSupply:
                case ComponentType.SSD:
                case ComponentType.CPUCooler:
                case ComponentType.HDD:
                case ComponentType.CaseFan:
                case ComponentType.GPU:
                case ComponentType.BuildService:
                case ComponentType.Miscellaneous:
                default:
                    return true;
                case ComponentType.OperatingSystem:
                case ComponentType.Plan:
                    return false;
            }

        }

        public bool AutoSearch()
        {
            switch (Type)
            {
                case ComponentType.CPU:
                case ComponentType.Motherboard:
                case ComponentType.RAM:
                case ComponentType.Case:
                case ComponentType.PowerSupply:
                case ComponentType.SSD:
                case ComponentType.CPUCooler:
                case ComponentType.HDD:
                case ComponentType.CaseFan:
                case ComponentType.GPU:
                case ComponentType.Miscellaneous:
                case ComponentType.Plan:
                default:
                    return false;
                case ComponentType.OperatingSystem:
                case ComponentType.BuildService:
                    return true;
            }

        }

        public static string CategoryFilterForType(ComponentType type)
        {
            //from microcenter.com search N field ex: &N=4294966995

            return type switch
            {
                ComponentType.CPU => "4294966995",
                ComponentType.Motherboard => "4294966996",
                ComponentType.RAM => "4294966965",
                ComponentType.Case => "4294964318",
                ComponentType.PowerSupply => "4294966654",
                ComponentType.GPU => "4294966938",
                ComponentType.SSD => "4294945779",
                ComponentType.HDD => "4294945772",
                ComponentType.CPUCooler => "4294966927",
                ComponentType.CaseFan => "4294966926",
                ComponentType.OperatingSystem => "4294967276",
                ComponentType.BuildService => "4294809201",
                _ => "",
            };
        }

        public static int MaxNumberPerType(ComponentType type)
        {
            switch (type)
            {
                case ComponentType.Plan:
                    return 0;
                case ComponentType.CPU:
                case ComponentType.Motherboard:
                case ComponentType.Case:
                case ComponentType.PowerSupply:
                case ComponentType.CPUCooler:
                case ComponentType.OperatingSystem:
                case ComponentType.BuildService:
                    return 1;
                case ComponentType.GPU:
                    return 4;
                case ComponentType.RAM:
                case ComponentType.CaseFan:
                    return 8;
                case ComponentType.SSD:
                    return 64;
                case ComponentType.HDD:
                case ComponentType.Miscellaneous:
                default:
                    return 64;
            }
        }

        public static string MCOLSelectorIDForType(ComponentType type)
        {
            switch (type)
            {
                case ComponentType.CPU:
                    return "1";
                case ComponentType.Motherboard:
                    return "2";
                case ComponentType.RAM:
                    return "7";
                case ComponentType.Case:
                    return "5";
                case ComponentType.PowerSupply:
                    return "6";
                case ComponentType.GPU:
                    return "8";
                case ComponentType.SSD:
                    return "15";
                case ComponentType.HDD:
                    return "14";
                case ComponentType.CPUCooler:
                    return "9";
                case ComponentType.CaseFan:
                    return "90";
                case ComponentType.BuildService:
                case ComponentType.Plan:
                case ComponentType.OperatingSystem:
                    return "12";
                case ComponentType.Miscellaneous:
                default:
                    return "19";
            }
        }
    }
}
