using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace micro_c_app.Models.Reference
{
    public class PlanReference
    {
        public static List<PlanReference> AllPlans = new List<PlanReference>();
        public PlanType Type { get; set; }
        public float MinPrice { get; set; }
        public float MaxPrice { get; set; }

        public List<(int duration, float price)> Tiers;

        public enum PlanType
        {
            Replacement,
            Small_Electronic_ADH,
            BYO_Replacement
        }

        public PlanReference(PlanType type, float minPrice, float maxPrice, params (int duration, float price)[] tiers)
        {
            Type = type;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            Tiers = tiers.ToList();
        }

        static PlanReference()
        {
            
             AllPlans.Add(new PlanReference(PlanType.Replacement, 0.00f,   4.99f,   (2, 0.75f) ,(3, 1.99f  )));
             AllPlans.Add(new PlanReference(PlanType.Replacement, 5.00f,   9.99f,   (2, 0.99f) ,(3, 2.49f  )));
             AllPlans.Add(new PlanReference(PlanType.Replacement, 10.00f,  14.99f,  (2, 1.49f) ,(3, 2.99f  )));
             AllPlans.Add(new PlanReference(PlanType.Replacement, 15.00f,  19.99f,  (2, 1.99f) ,(3, 3.99f  )));
             AllPlans.Add(new PlanReference(PlanType.Replacement, 20.00f,  24.99f,  (2, 2.49f) ,(3, 4.99f  )));
             AllPlans.Add(new PlanReference(PlanType.Replacement, 25.00f,  49.99f,  (2, 4.99f) ,(3, 9.99f  )));
             AllPlans.Add(new PlanReference(PlanType.Replacement, 50.00f,  74.99f,  (2, 6.99f) ,(3, 14.99f )));
             AllPlans.Add(new PlanReference(PlanType.Replacement, 75.00f,  99.99f,  (2, 9.99f) ,(3, 19.99f )));
             AllPlans.Add(new PlanReference(PlanType.Replacement, 100.00f, 199.99f, (2, 19.99f),(3, 39.99f )));
             AllPlans.Add(new PlanReference(PlanType.Replacement, 200.00f, 299.99f, (2, 29.99f),(3, 59.99f )));
             AllPlans.Add(new PlanReference(PlanType.Replacement, 300.00f, 399.99f, (2, 49.99f),(3, 89.99f )));
             AllPlans.Add(new PlanReference(PlanType.Replacement, 400.00f, 500.00f, (2, 69.99f),(3, 139.99f)));
             
             AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 0.00f,    49.99f,   (1, 5.99f),   (2, 14.99f)));
             AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 50.00f,   99.99f,   (1, 19.99f),  (2, 39.99f)));
             AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 100.00f,  199.99f,  (1, 29.99f),  (2, 69.99f)));
             AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 200.00f,  299.99f,  (1, 49.99f),  (2, 99.99f)));
             AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 300.00f,  399.99f,  (1, 59.99f),  (2, 139.99f)));
             AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 400.00f,  499.99f,  (1, 79.99f),  (2, 179.99f)));
             AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 500.00f,  749.99f,  (1, 99.99f),  (2, 199.99f)));
             AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 750.00f,  999.99f,  (1, 199.99f), (2, 299.99f)));
             AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 1000.00f, 1499.99f, (1, 299.99f), (2, 399.99f)));
             
             AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 0.00f,    49.99f,   (2, 6.99f),   (3, 9.99f)));
             AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 50.00f,   99.99f,   (2, 14.99f),  (3, 29.99f)));
             AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 100.00f,  199.99f,  (2, 29.99f),  (3, 49.99f)));
             AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 200.00f,  299.99f,  (2, 49.99f),  (3, 69.99f)));
             AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 300.00f,  399.99f,  (2, 69.99f),  (3, 99.99f)));
             AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 400.00f,  499.99f,  (2, 89.99f),  (3, 129.99f)));
             AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 500.00f,  999.99f,  (2, 139.99f), (3, 199.99f)));
             AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 1000.00f, 1499.99f, (2, 199.99f), (3, 279.99f)));
             AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 1500.00f, 3000.00f, (2, 299.99f), (3, 429.99f)));
        }
    }
}
