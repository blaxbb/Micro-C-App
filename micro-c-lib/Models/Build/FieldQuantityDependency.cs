using micro_c_lib.Models.Build;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MicroCLib.Models
{
    public class FieldQuantityDependency : BuildComponentDependency
    {
        public BuildComponent.ComponentType subType { get; set; }
        public BuildComponent.ComponentType containerType { get; set; }

        public string subTypeFieldName { get; set; } = "";
        public string containerFieldName { get; set; } = "";

        public FieldQuantityDependency(BuildComponent.ComponentType subItem, BuildComponent.ComponentType containerItem, string containerField, string subField = null)
        {
            subType = subItem;
            containerType = containerItem;
            containerFieldName = containerField;
            subTypeFieldName = subField;
        }

        public override string HintText(List<Item> items, BuildComponent.ComponentType type)
        {
            if(type == subType)
            {
                var containerItems = items.Where(i => i.ComponentType == containerType);
                if(containerItems.Count() == 0)
                {
                    return null;
                }

                var containerSlots = containerItems.Sum(i => GetValue(i, containerFieldName));

                return $"{containerType} has {containerSlots} available for {subType}";
            }
            if(type == containerType)
            {
                var subItems = items.Where(i => i.ComponentType == subType);
                if(subItems.Count() == 0)
                {
                    return null;
                }

                int subItemCount;

                if (!string.IsNullOrWhiteSpace(subTypeFieldName))
                {
                    subItemCount = subItems.Sum(i => GetValue(i, subTypeFieldName));
                }
                else
                {
                    subItemCount = subItems.Count();
                }

                return $"{containerType} must have support for {subItemCount} {subType}";
            }

            return null;
        }

        public override List<DependencyResult> HasErrors(List<Item> items)
        {
            var containerItems = items.Where(i => i.ComponentType == containerType);
            var subItems = items.Where(i => i.ComponentType == subType);

            var containerSlots = containerItems.Sum(i => GetValue(i, containerFieldName));
            int subItemCount;

            if(!string.IsNullOrWhiteSpace(subTypeFieldName))
            {
                subItemCount = subItems.Sum(i => GetValue(i, subTypeFieldName));
            }
            else
            {
                subItemCount = subItems.Count();
            }


            if(subItemCount > containerSlots)
            {
                return containerItems.Concat(subItems)
                    .Select(i =>
                        new DependencyResult(i, $"{containerType} must have at least {subItemCount} {subType}")
                    ).ToList();
            }


            return new List<DependencyResult>();
        }

        private static int GetValue(Item item, string field)
        {
            if(item == null || item.Specs == null || !item.Specs.ContainsKey(field))
            {
                return 0;
            }

            var spec = item.Specs[field];
            var specNumber = Regex.Match(spec, "([\\d\\.]+)").Groups[1].Value;
            if (int.TryParse(specNumber, out int val))
            {
                return val;
            }

            return 0;
        }

    }
}