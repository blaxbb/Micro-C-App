using micro_c_lib.Models.Build;
using System.Collections.Generic;
using static MicroCLib.Models.BuildComponent;

namespace MicroCLib.Models
{
    public abstract class BuildComponentDependency : IBuildComponentDependency
    {
        public static List<BuildComponentDependency> Dependencies { get; }

        //public abstract string HintText();
        public abstract List<DependencyResult> HasErrors(List<Item> items);
        public abstract string? HintText(List<Item> items, ComponentType type);
        public string Name { get; protected set; }

        public BuildComponentDependency(string name)
        {
            Name = name;
        }

        static BuildComponentDependency()
        {
            Dependencies = new List<BuildComponentDependency>
            {
                //CPU -> Other
                new FieldContainsDependency("CPU Socket", ComponentType.CPU, "Socket Type", ComponentType.Motherboard, "Socket Type"),
                new FieldContainsDependency("CPU Chipset", ComponentType.CPU, "Compatibility", ComponentType.Motherboard, "North Bridge"),

                //Mobo -> Other
                new FieldContainsDependency("Motherboard Memory Speed", ComponentType.Motherboard, "Memory Type", ComponentType.RAM, "Memory Speed (MHz)"),
                new FieldContainsDependency("Motherboard Form Factor", ComponentType.Motherboard, "Form Factor", ComponentType.Case, "Motherboard Support"),
                new FieldQuantityDependency("Motherboard RAM Slots", ComponentType.RAM, ComponentType.Motherboard, "Memory Slots", "Number of Modules"),
                new FieldComparisonDependency("Motherboard Max Memory", ComponentType.Motherboard, "Max Memory Supported", ComponentType.RAM, "Memory Capacity", FieldComparisonDependency.CompareMode.GreaterThanOrEqual){
                    FailOnEmpty = false
                },
                new FieldQuantityDependency("Has OS", ComponentType.OperatingSystem, ComponentType.Motherboard),

                new FieldContainsDependency("M.2 form factor", ComponentType.Motherboard, "M.2 Port Type", ComponentType.SSD, "Form Factor"),

                //Case -> Other
                new FieldComparisonDependency("Case GPU Length", ComponentType.Case, "Max Video Card Length", ComponentType.GPU, "Video Card Length", FieldComparisonDependency.CompareMode.GreaterThanOrEqual),
                new FieldComparisonDependency("Case CPU Heatsink Height", ComponentType.Case, "Max CPU Heatsink Height", ComponentType.CPUCooler, "Heatsink Height", FieldComparisonDependency.CompareMode.GreaterThanOrEqual),
                new FieldComparisonDependency("Case PSU Max Depth", ComponentType.Case, "Max Power Supply Depth", ComponentType.PowerSupply, "Power Supply Depth", FieldComparisonDependency.CompareMode.GreaterThanOrEqual),

                new FieldComparisonDependency("GPU Recommended PSU", ComponentType.GPU, "Recommended Power Supply", ComponentType.PowerSupply, "Wattage", FieldComparisonDependency.CompareMode.LessThanOrEqual),

                new FieldQuantityDependency("Case 3.5\" Drive Bay Quantity", ComponentType.HDD, ComponentType.Case, "Internal 3.5\" Drive Bays")
            };
            //Rad size supported in case
        }
    }
}