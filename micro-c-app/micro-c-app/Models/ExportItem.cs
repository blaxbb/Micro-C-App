using System;
using System.Collections.Generic;
using System.Text;

namespace micro_c_app.Models
{
    public class ExportItem
    {
        public string SKU { get; set; }
        public int QTY { get; set; }

        public ExportItem(Item i)
        {
            SKU = i.SKU;
            QTY = i.Quantity;
        }
    }
}
