using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace micro_c_app.Models
{
    public class BuildComponentDependency
    {
        public static List<BuildComponentDependency> Dependencies { get; }
        public BuildComponent First { get; set; }
        public BuildComponent Second { get; set; }
        public BuildComponent.ComponentType FirstType { get; set; }
        public BuildComponent.ComponentType SecondType { get; set; }
        public string FirstFieldName { get; set; }
        public string SecondFieldName { get; set; }

        public string FirstValue => First.Item?.Specs?[FirstFieldName];
        public string SecondValue => Second.Item?.Specs?[SecondFieldName];

        public string ErrorText => $"({FirstFieldName}) {FirstValue.Replace('\n', ',')} != ({SecondFieldName}) {SecondValue.Replace('\n', ',')}";

        public string HintText()
        {
            if(First?.Item == null && Second?.Item == null)
            {
                return null;
            }
            if(First == null)
            {
                return $"{FirstFieldName} = {SecondValue.Replace('\n', ',')}";
            }
            else
            {
                return $"{SecondFieldName} = {FirstValue.Replace('\n', ',')}";
            }
        }

        public override string ToString()
        {
            return $"{FirstType}({FirstFieldName}) === {SecondType}({SecondFieldName})";
        }

        static BuildComponentDependency()
        {
            Dependencies = new List<BuildComponentDependency>();

            AddCPUDependencies();
            AddMoboDependencies();
        }

        static void AddCPUDependencies()
        {
            Dependencies.Add(new BuildComponentDependency()
            {
                FirstType = BuildComponent.ComponentType.CPU,
                SecondType = BuildComponent.ComponentType.Motherboard,
                FirstFieldName = "Socket Type",
                SecondFieldName = "Socket Type"
            });

            Dependencies.Add(new BuildComponentDependency()
            {
                FirstType = BuildComponent.ComponentType.CPU,
                SecondType = BuildComponent.ComponentType.Motherboard,
                FirstFieldName = "Compatibility",
                SecondFieldName = "North Bridge"
            });
        }

        static void AddMoboDependencies()
        {
            Dependencies.Add(new BuildComponentDependency()
            {
                FirstType = BuildComponent.ComponentType.Motherboard,
                SecondType = BuildComponent.ComponentType.RAM,
                FirstFieldName = "Memory Type",
                SecondFieldName = "Memory Speed (MHz)"
            });
        }

        public BuildComponent Other(BuildComponent comp) => First == comp ? Second : First;

        public bool Applicable(BuildComponent first, BuildComponent second)
        {
            return (FirstType == first.Type && SecondType == second.Type) ||
                    (FirstType == second.Type && SecondType == first.Type);
        }

        public bool Compatible()
        {
            return Compatible(First, Second);
        }

        public bool Compatible(BuildComponent a, BuildComponent b)
        {
            if(a == null || b == null)
            {
                return false;
            }

            BuildComponent first, second;
            if(a.Type == FirstType && b.Type == SecondType)
            {
                first = a;
                second = b;
            }
            else if(a.Type == SecondType && b.Type == FirstType)
            {
                first = b;
                second = a;
            }
            else
            {
                return true;
            }

            if(a.Item == null || b.Item == null)
            {
                return true;
            }

            if ((!first.Item?.Specs.ContainsKey(FirstFieldName)) ?? true)
            {
                return false;
            }

            if ((!second.Item?.Specs.ContainsKey(SecondFieldName)) ?? true)
            {
                return true;
            }

            if (FirstValue == null || SecondValue == null)
            {
                return true;
            }

            var secondSpecLines = SecondValue.Split('\n');
            return FirstValue.Split('\n').Any(s => secondSpecLines.Contains(s));
        }
    }
}
