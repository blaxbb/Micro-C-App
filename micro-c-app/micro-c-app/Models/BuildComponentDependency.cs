using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace micro_c_app.Models
{
    public class BuildComponentDependency
    {
        public static List<BuildComponentDependency> Dependencies { get; }
        public BuildComponent.ComponentType FirstType { get; set; }
        public BuildComponent.ComponentType SecondType { get; set; }
        public string FirstFieldName { get; set; }
        public string SecondFieldName { get; set; }

        public string ErrorText => $"{FirstType}-{FirstFieldName} != {SecondType}-{SecondFieldName}";

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

        public bool Applicable(BuildComponent first, BuildComponent second)
        {
            return (FirstType == first.Type && SecondType == second.Type) ||
                    (FirstType == second.Type && SecondType == first.Type);
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

            if ((!first.Item?.Specs.ContainsKey(FirstFieldName)) ?? true)
            {
                return false;
            }

            if ((!second.Item?.Specs.ContainsKey(SecondFieldName)) ?? true)
            {
                return true;
            }

            var firstSepc = first.Item?.Specs?[FirstFieldName];
            var secondSpec = second.Item?.Specs?[SecondFieldName];
            if (firstSepc == null || secondSpec == null)
            {
                return true;
            }

            var secondSpecLines = secondSpec.Split('\n');
            return firstSepc.Split('\n').Any(s => secondSpecLines.Contains(s));
        }
    }
}
