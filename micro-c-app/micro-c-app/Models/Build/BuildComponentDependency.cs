using System.Collections.Generic;
using static micro_c_app.Models.BuildComponent;

namespace micro_c_app.Models
{
    public abstract class BuildComponentDependency
    {
        public static List<BuildComponentDependency> Dependencies { get; }
        public virtual string ErrorText => "";
        public BuildComponent First { get; set; }
        public BuildComponent Second { get; set; }
        public BuildComponent.ComponentType FirstType { get; set; }
        public BuildComponent.ComponentType SecondType { get; set; }

        public string FirstFieldName { get; set; }
        public string SecondFieldName { get; set; }

        public string FirstValue => First.Item?.Specs?[FirstFieldName];
        public string SecondValue => Second.Item?.Specs?[SecondFieldName];

        public bool Applicable(BuildComponent first, BuildComponent second)
        {
            return (FirstType == first.Type && SecondType == second.Type) ||
                    (FirstType == second.Type && SecondType == first.Type);
        }
        public abstract BuildComponentDependency Clone();
        public bool Compatible()
        {
            return Compatible(First, Second);
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
        public BuildComponent Other(BuildComponent comp) => First == comp ? Second : First;

        static BuildComponentDependency()
        {
            Dependencies = new List<BuildComponentDependency>();

            //CPU -> Other
            Dependencies.Add(new FieldContainsDependency(ComponentType.CPU, "Socket Type", ComponentType.Motherboard, "Socket Type"));
            Dependencies.Add(new FieldContainsDependency(ComponentType.CPU, "Compatibility", ComponentType.Motherboard, "North Bridge"));

            //Mobo -> Other
            Dependencies.Add(new FieldContainsDependency(ComponentType.Motherboard, "Memory Type", ComponentType.RAM, "Memory Speed (MHz)"));
            Dependencies.Add(new FieldContainsDependency(ComponentType.Motherboard, "Form Factor", ComponentType.Case, "Motherboard Support"));
            //Mobo liquid cooler compatibility
            Dependencies.Add(new FieldContainsDependency(ComponentType.Motherboard, "M.2 Port Type", ComponentType.SSD, "Form Factor"));

            Dependencies.Add(new FieldComparisonDependency(ComponentType.GPU, "Recommended Power Supply", ComponentType.PowerSupply, "Wattage", FieldComparisonDependency.CompareMode.LessThanOrEqual));

            Dependencies.Add(new FieldQuantityDependency(ComponentType.HDD, ComponentType.Case, "Internal 3.5\" Drive Bays"));
            //Rad size supported in case
        }
    }
}