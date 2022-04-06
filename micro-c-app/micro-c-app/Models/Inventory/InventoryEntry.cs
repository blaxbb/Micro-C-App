using System;
using System.Collections.Generic;
using System.Text;

namespace micro_c_app.Models.Inventory
{
    internal class InventoryEntry
    {
        public long Id { get; set; }
        public string Sku { get; set; }
        public InventoryLocation Location { get; set; }
        public DateTime Created { get; set; }
        public string? Author { get; set; }
    }
}
