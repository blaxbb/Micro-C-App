using System;
using System.Collections.Generic;
using System.Linq;
using static micro_c_app.Models.BuildComponent;

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
            if (First?.Item == null && Second?.Item == null)
            {
                return null;
            }
            if (First.Item == null)
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

        public BuildComponentDependency Clone()
        {
            return new BuildComponentDependency(FirstType, FirstFieldName, SecondType, SecondFieldName);
        }

        static BuildComponentDependency()
        {
            Dependencies = new List<BuildComponentDependency>();

            //CPU -> Other
            Dependencies.Add(new BuildComponentDependency(ComponentType.CPU, "Socket Type", ComponentType.Motherboard, "Socket Type"));
            Dependencies.Add(new BuildComponentDependency(ComponentType.CPU, "Compatibility", ComponentType.Motherboard, "North Bridge"));

            //Mobo -> Other
            Dependencies.Add(new BuildComponentDependency(ComponentType.Motherboard, "Memory Type", ComponentType.RAM, "Memory Speed (MHz)"));
            Dependencies.Add(new BuildComponentDependency(ComponentType.Motherboard, "Form Factor", ComponentType.Case, "Motherboard Support"));
            //Mobo liquid cooler compatibility
            //SSD Form Factor

            //GPU Power supply Wattage

            //HDD 3.5" exists in case
            //Rad size supported in case
        }

        public BuildComponentDependency()
        {

        }

        public BuildComponentDependency(ComponentType first, string firstField, ComponentType second, string secondField)
        {
            FirstType = first;
            FirstFieldName = firstField;
            SecondType = second;
            SecondFieldName = secondField;
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
            if (a == null || b == null)
            {
                return false;
            }

            BuildComponent first, second;
            if (a.Type == FirstType && b.Type == SecondType)
            {
                first = a;
                second = b;
            }
            else if (a.Type == SecondType && b.Type == FirstType)
            {
                first = b;
                second = a;
            }
            else
            {
                return true;
            }

            if (a.Item == null || b.Item == null)
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
