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

        static BuildComponentDependency()
        {
            Dependencies = new List<BuildComponentDependency>
            {
                //CPU -> Other
                new FieldContainsDependency(ComponentType.CPU, "Socket Type", ComponentType.Motherboard, "Socket Type"),
                new FieldContainsDependency(ComponentType.CPU, "Compatibility", ComponentType.Motherboard, "North Bridge"),

                //Mobo -> Other
                new FieldContainsDependency(ComponentType.Motherboard, "Memory Type", ComponentType.RAM, "Memory Speed (MHz)"),
                new FieldContainsDependency(ComponentType.Motherboard, "Form Factor", ComponentType.Case, "Motherboard Support"),
                new FieldQuantityDependency(ComponentType.RAM, ComponentType.Motherboard, "Memory Slots", "Number of Modules"),
                new FieldComparisonDependency(ComponentType.Motherboard, "Max Memory Supported", ComponentType.RAM, "Memory Capacity", FieldComparisonDependency.CompareMode.GreaterThanOrEqual),
                new FieldQuantityDependency(ComponentType.OperatingSystem, ComponentType.Motherboard),

                new FieldContainsDependency(ComponentType.Motherboard, "M.2 Port Type", ComponentType.SSD, "Form Factor"),

                //Case -> Other
                new FieldComparisonDependency(ComponentType.Case, "Max Video Card Length", ComponentType.GPU, "Video Card Length", FieldComparisonDependency.CompareMode.GreaterThanOrEqual),
                new FieldComparisonDependency(ComponentType.Case, "Max CPU Heatsink Height", ComponentType.CPUCooler, "Heatsink Height", FieldComparisonDependency.CompareMode.GreaterThanOrEqual),
                new FieldComparisonDependency(ComponentType.Case, "Max Power Supply Depth", ComponentType.PowerSupply, "Power Supply Depth", FieldComparisonDependency.CompareMode.GreaterThanOrEqual),

                new FieldComparisonDependency(ComponentType.GPU, "Recommended Power Supply", ComponentType.PowerSupply, "Wattage", FieldComparisonDependency.CompareMode.LessThanOrEqual),

                new FieldQuantityDependency(ComponentType.HDD, ComponentType.Case, "Internal 3.5\" Drive Bays")
            };
            //Rad size supported in case
        }
    }
}