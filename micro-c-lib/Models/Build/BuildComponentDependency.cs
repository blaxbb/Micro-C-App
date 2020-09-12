using System.Collections.Generic;
using static MicroCLib.Models.BuildComponent;
using static MicroCLib.Models.BuildComponent;

namespace MicroCLib.Models
{
    public abstract class BuildComponentDependency
    {
        public static List<BuildComponentDependency> Dependencies { get; }
        public virtual string ErrorText => "";
        public BuildComponent? First { get; set; }
        public BuildComponent? Second { get; set; }
        public BuildComponent.ComponentType FirstType { get; set; }
        public BuildComponent.ComponentType SecondType { get; set; }

        public string FirstFieldName { get; set; } = "";
        public string SecondFieldName { get; set; } = "";

        public string? FirstValue => First?.Item?.Specs?[FirstFieldName];
        public string? SecondValue => Second?.Item?.Specs?[SecondFieldName];

        public bool Applicable(BuildComponent first, BuildComponent second)
        {
            return (FirstType == first.Type && SecondType == second.Type) ||
                    (FirstType == second.Type && SecondType == first.Type);
        }
        public abstract BuildComponentDependency Clone();
        public bool Compatible()
        {
            return Compatible(First!, Second!);
        }
        public abstract bool Compatible(BuildComponent a, BuildComponent b);
        public bool SetRelevant(BuildComponent comp)
        {
            if (comp.Type == FirstType)
            {
                First = comp;
            }
            else if (comp.Type == SecondType)
            {
                Second = comp;
            }
            else
            {
                return false;
            }

            return true;
        }

        public abstract string HintText();
        public BuildComponent? Other(BuildComponent comp) => First == comp ? Second : First;

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

                new FieldContainsDependency(ComponentType.Motherboard, "M.2 Port Type", ComponentType.SSD, "Form Factor"),

                new FieldComparisonDependency(ComponentType.GPU, "Recommended Power Supply", ComponentType.PowerSupply, "Wattage", FieldComparisonDependency.CompareMode.LessThanOrEqual),

                new FieldQuantityDependency(ComponentType.HDD, ComponentType.Case, "Internal 3.5\" Drive Bays")
            };
            //Rad size supported in case
        }
    }
}