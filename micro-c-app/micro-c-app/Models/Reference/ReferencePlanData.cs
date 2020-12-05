using MicroCLib.Models.Reference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MicroCLib.Models.Reference.PlanReference;

namespace micro_c_app.Models.Reference
{
    public class ReferencePlanData : IReferenceItem
    {
        public string Name { get; set; }
        public List<PlanReference> Plans { get; set; }

        public ReferencePlanData()
        {
            Plans = new List<PlanReference>();
        }

        public ReferencePlanData(PlanType type)
        {
            Name = type.FriendlyName();
            Plans = AllPlans.Where(p => p.Type == type).ToList();
        }
    }
}
