using System;
using System.Collections.Generic;
using System.Text;

namespace micro_c_app.Models.Reference
{
    public class ReferenceTree : IReferenceItem
    {
        public string Name { get; set; }
        public List<IReferenceItem> Nodes { get; set; }

        public ReferenceTree()
        {
            Nodes = new List<IReferenceItem>();
        }

        public ReferenceTree(string name) : this()
        {
            Name = name;
        }
    }
}
