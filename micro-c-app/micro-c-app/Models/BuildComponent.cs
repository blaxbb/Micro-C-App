using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace micro_c_app.Models
{
    public class BuildComponent : NotifyPropertyChangedItem
    {
        private string sku;
        private string productID;

        public enum ComponentType
        {
            CPU,
            Motherboard,
            RAM
        }
        public ComponentType Type { get; set; }

        public string SKU { get => sku; set { SetProperty(ref sku, value); OnPropertyChanged(nameof(ComponentLabel)); } }
        public string ProductID { get => productID; set => SetProperty(ref productID, value); }

        public string ComponentLabel => $"{Type.ToString()} - {SKU}";
    }
}
