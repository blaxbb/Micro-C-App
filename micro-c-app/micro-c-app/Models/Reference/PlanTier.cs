using System;
using System.Collections.Generic;
using System.Text;

namespace micro_c_app.Models.Reference
{
    public class PlanTier
    {
        public int Duration { get; set; }
        public float Price { get; set; }

        public PlanTier(int duration, float price)
        {
            Duration = duration;
            Price = price;
        }
    }
}
