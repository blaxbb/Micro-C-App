using System;
using System.Collections.Generic;
using System.Text;
using static MicroCLib.Models.BuildComponent;

namespace micro_c_app.Models
{
    public class ComponentTypeInfo
    {
        public ComponentType Type { get; set; }
        public string Name { get; set; }
        public string SearchCategory { get; set; }
        public string Icon { get; set; }

        public ComponentTypeInfo(ComponentType type)
        {
            Type = type;
            Name = Enum.GetName(typeof(ComponentType), type);
            SearchCategory = CategoryFilterForType(type);
            Icon = "icon";
            Icon = "\uf2db";
        }

        public ComponentTypeInfo(ComponentType type, string icon)
        {
            Type = type;
            Name = Enum.GetName(typeof(ComponentType), type);
            SearchCategory = CategoryFilterForType(type);
            Icon = icon;
        }
    }
}
