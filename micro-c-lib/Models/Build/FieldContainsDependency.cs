using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using static MicroCLib.Models.BuildComponent;

namespace MicroCLib.Models
{
    public class FieldContainsDependency : BuildComponentDependency
    {
        [JsonIgnore]
        public override string ErrorText => $"({FirstFieldName}) {FirstValue?.Replace('\n', ',')} != ({SecondFieldName}) {SecondValue?.Replace('\n', ',')}";

        public override string HintText()
        {
            if (First?.Item == null && Second?.Item == null)
            {
                return "";
            }
            if (First?.Item == null)
            {
                return $"{FirstFieldName} = {SecondValue?.Replace('\n', ',') ?? ""}";
            }
            else
            {
                return $"{SecondFieldName} = {FirstValue?.Replace('\n', ',') ?? ""}";
            }
        }

        public override string ToString()
        {
            return $"{FirstType}({FirstFieldName}) === {SecondType}({SecondFieldName})";
        }

        public override BuildComponentDependency Clone()
        {
            return new FieldContainsDependency(FirstType, FirstFieldName, SecondType, SecondFieldName);
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

        public override bool Compatible(BuildComponent a, BuildComponent b)
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
            return FirstValue.Split('\n').Any(s => secondSpecLines.Any(l => l.Contains(s)));
        }
    }
}
