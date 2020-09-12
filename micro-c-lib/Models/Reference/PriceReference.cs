using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroCLib.Models
{
    public class PriceReference
    {
        public string Path { get; set; }
        public List<(string name, float price)> Items { get; set; }

        public PriceReference()
        {

        }

        public PriceReference(string path, params (string name, float price)[] items)
        {
            Path = path;
            Items = items.ToList();
        }
    }
}
