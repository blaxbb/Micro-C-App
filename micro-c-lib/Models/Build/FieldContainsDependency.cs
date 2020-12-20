using micro_c_lib.Models.Build;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MicroCLib.Models.BuildComponent;

namespace MicroCLib.Models
{
    public class FieldContainsDependency : BuildComponentDependency
    {
        //public override string HintText()
        //{
        //    if (First?.Item == null && Second?.Item == null)
        //    {
        //        return "";
        //    }
        //    if (First?.Item == null)
        //    {
        //        return $"{FirstFieldName} = {SecondValue?.Replace('\n', ',') ?? ""}";
        //    }
        //    else
        //    {
        //        return $"{SecondFieldName} = {FirstValue?.Replace('\n', ',') ?? ""}";
        //    }
        //}
        public BuildComponent.ComponentType FirstType { get; set; }
        public BuildComponent.ComponentType SecondType { get; set; }

        public string FirstFieldName { get; set; } = "";
        public string SecondFieldName { get; set; } = "";

        public override string ToString()
        {
            return $"{FirstType}({FirstFieldName}) === {SecondType}({SecondFieldName})";
        }

        public FieldContainsDependency()
        {

        }

        public FieldContainsDependency(ComponentType first, string firstField, ComponentType second, string secondField)
        {
            FirstType = first;
            FirstFieldName = firstField;
            SecondType = second;
            SecondFieldName = secondField;
        }

        public override List<DependencyResult> HasErrors(List<Item> items)
        {
            var errors = new List<DependencyResult>();
            for (int i = 0; i < items.Count; i++)
            {
                var primary = items[i];
                if (primary.ComponentType == FirstType || primary.ComponentType == SecondType)
                {
                    for (int j = i + 1; j < items.Count; j++)
                    {
                        var secondary = items[j];
                        if ((secondary.ComponentType == FirstType || secondary.ComponentType == SecondType) && secondary.ComponentType != primary.ComponentType)
                        {
                            if(!Compatible(primary, secondary))
                            {
                                errors.Add(new DependencyResult(primary, $"{GetValue(primary, FirstFieldName)} != {GetValue(secondary, SecondFieldName)} on {secondary.Name}"));
                                errors.Add(new DependencyResult(secondary, $"{GetValue(secondary, SecondFieldName)} != {GetValue(primary, FirstFieldName)} on {primary.Name}"));
                            }
                        }
                    }
                }
            }
            return errors;
        }

        public override string? HintText(List<Item> items, ComponentType type)
        {
            if(type == FirstType)
            {
                var hints = items.Where(i => i.ComponentType == SecondType).Select(i => $"Must have {FirstFieldName} = {SecondFieldName} ({GetValue(i, SecondFieldName)})");
                return string.Join("\n", hints);
            }
            if (type == SecondType)
            {
                var hints = items.Where(i => i.ComponentType == FirstType).Select(i => $"Must have {SecondFieldName} = {FirstFieldName} ({GetValue(i, FirstFieldName)})");
                return string.Join("\n", hints);
            }

            return null;
        }

        private static string GetValue(Item item, string field)
        {
            if (item == null || item.Specs == null || !item.Specs.ContainsKey(field))
            {
                return null;
            }

            return item.Specs[field];
        }

        public bool Compatible(Item a, Item b)
        {
            if (a == null || b == null)
            {
                return true;
            }

            Item first, second;
            if (a.ComponentType == FirstType && b.ComponentType == SecondType)
            {
                first = a;
                second = b;
            }
            else if (a.ComponentType == SecondType && b.ComponentType == FirstType)
            {
                first = b;
                second = a;
            }
            else
            {
                return true;
            }

            if (!first.Specs.ContainsKey(FirstFieldName))
            {
                return true;
            }

            if (!second.Specs.ContainsKey(SecondFieldName))
            {
                return true;
            }

            string firstValue = first.Specs[FirstFieldName];
            string secondValue = second.Specs[SecondFieldName];

            if (firstValue == null || secondValue == null)
            {
                return true;
            }

            var secondSpecLines = secondValue.Split('\n');
            return firstValue.Split('\n').Any(s => secondSpecLines.Any(l => l.Contains(s)));
        }
    }
}
