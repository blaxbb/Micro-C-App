using System;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace micro_c_app.Models
{
    internal class FieldComparisonDependency : BuildComponentDependency
    {
        public CompareMode Mode { get; set; }

        [JsonIgnore]
        public override string ErrorText => $"{FirstFieldName} {Mode} {SecondFieldName}";

        public enum CompareMode
        {
            LessThan,
            LessThanOrEqual,
            Equal,
            GreaterThanOrEqual,
            GreaterThan
        }
        public FieldComparisonDependency(BuildComponent.ComponentType first, string fieldFirst, BuildComponent.ComponentType second, string fieldSecond, CompareMode mode)
        {
            FirstType = first;
            SecondType = second;
            FirstFieldName = fieldFirst;
            SecondFieldName = fieldSecond;
            Mode = mode;
        }

        public override BuildComponentDependency Clone()
        {
            return new FieldComparisonDependency(FirstType, FirstFieldName, SecondType, SecondFieldName, Mode);
        }

        public override bool Compatible(BuildComponent a, BuildComponent b)
        {
            if(a.Item == null || b.Item == null)
            {
                return true;
            }

            if(SecondValue == null || FirstValue == null)
            {
                return false;
            }

            var m1 = Regex.Match(FirstValue, "([\\d\\.]+)").Groups[1].Value;
            var m2 = Regex.Match(SecondValue, "([\\d\\.]+)").Groups[1].Value;

            if(float.TryParse(m1, out float f1) && float.TryParse(m2, out float f2))
            {
                switch (Mode)
                {
                    case CompareMode.LessThan:
                        return f1 < f2;
                    case CompareMode.LessThanOrEqual:
                        return f1 <= f2;
                    case CompareMode.Equal:
                        return f1 == f2;
                    case CompareMode.GreaterThanOrEqual:
                        return f1 >= f2;
                    case CompareMode.GreaterThan:
                        return f1 > f2;
                }
            }

            return false;
        }

        public override string HintText()
        {
            return $"{FirstFieldName} {Mode} {SecondFieldName}";
        }
    }
}