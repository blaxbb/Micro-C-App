using System.Text.Json.Serialization;

namespace micro_c_app.Models
{
    internal class FieldQuantityDependency : BuildComponentDependency
    {
        [JsonIgnore]
        public override string ErrorText => $"{FirstType} requires {SecondType} to have {SecondFieldName}";

        public FieldQuantityDependency(BuildComponent.ComponentType first, BuildComponent.ComponentType second, string field)
        {
            FirstType = first;
            SecondType = second;
            SecondFieldName = field;
        }

        public override BuildComponentDependency Clone()
        {
            return new FieldQuantityDependency(FirstType, SecondType, SecondFieldName);
        }

        public override bool Compatible(BuildComponent a, BuildComponent b)
        {
            if(a?.Item == null || b?.Item == null)
            {
                return true;
            }

            var value = SecondValue;
            if(SecondValue == null)
            {
                return false;
            }

            if(int.TryParse(value, out int count))
            {
                if(count > 0)
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        public override string HintText()
        {
            return $"{FirstType} requires {SecondType} to have {SecondFieldName}";
        }
    }
}