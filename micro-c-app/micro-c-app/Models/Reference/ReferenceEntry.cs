using System;
using System.Collections.Generic;
using System.Text;

namespace micro_c_app.Models.Reference
{
    public class ReferenceEntry : IReferenceItem
    {
        public string Name { get; set; }
        public string Data { get; set; }
    }
}
