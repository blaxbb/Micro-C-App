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
        public string Name => Type.ToString().Replace('_', ' ');
        public float MinPrice { get; set; }
        public float MaxPrice { get; set; }

        public List<PlanTier> Tiers;

        public enum PlanType
        {
            Replacement,
            Small_Electronic_ADH,
            BYO_Replacement,
            Build_Plan,
        }

        public PlanReference(PlanType type, float minPrice, float maxPrice, params PlanTier[] tiers)
        {
            Type = type;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            Tiers = tiers.ToList();
        }

        public static PlanReference Get(PlanType type, float price)
        {
            return AllPlans.FirstOrDefault(p => p.Type == type && p.MinPrice <= price && p.MaxPrice >= price);
        }

        static PlanReference()
        {

            AllPlans.Add(new PlanReference(PlanType.Replacement, 0.00f, 4.99f, new PlanTier(2, 0.75f), new PlanTier(3, 1.99f)));
            AllPlans.Add(new PlanReference(PlanType.Replacement, 5.00f, 9.99f, new PlanTier(2, 0.99f), new PlanTier(3, 2.49f)));
            AllPlans.Add(new PlanReference(PlanType.Replacement, 10.00f, 14.99f, new PlanTier(2, 1.49f), new PlanTier(3, 2.99f)));
            AllPlans.Add(new PlanReference(PlanType.Replacement, 15.00f, 19.99f, new PlanTier(2, 1.99f), new PlanTier(3, 3.99f)));
            AllPlans.Add(new PlanReference(PlanType.Replacement, 20.00f, 24.99f, new PlanTier(2, 2.49f), new PlanTier(3, 4.99f)));
            AllPlans.Add(new PlanReference(PlanType.Replacement, 25.00f, 49.99f, new PlanTier(2, 4.99f), new PlanTier(3, 9.99f)));
            AllPlans.Add(new PlanReference(PlanType.Replacement, 50.00f, 74.99f, new PlanTier(2, 6.99f), new PlanTier(3, 14.99f)));
            AllPlans.Add(new PlanReference(PlanType.Replacement, 75.00f, 99.99f, new PlanTier(2, 9.99f), new PlanTier(3, 19.99f)));
            AllPlans.Add(new PlanReference(PlanType.Replacement, 100.00f, 199.99f, new PlanTier(2, 19.99f), new PlanTier(3, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Replacement, 200.00f, 299.99f, new PlanTier(2, 29.99f), new PlanTier(3, 59.99f)));
            AllPlans.Add(new PlanReference(PlanType.Replacement, 300.00f, 399.99f, new PlanTier(2, 49.99f), new PlanTier(3, 89.99f)));
            AllPlans.Add(new PlanReference(PlanType.Replacement, 400.00f, 500.00f, new PlanTier(2, 69.99f), new PlanTier(3, 139.99f)));

            AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 0.00f, 49.99f, new PlanTier(1, 5.99f), new PlanTier(2, 14.99f)));
            AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 50.00f, 99.99f, new PlanTier(1, 19.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 100.00f, 199.99f, new PlanTier(1, 29.99f), new PlanTier(2, 69.99f)));
            AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 200.00f, 299.99f, new PlanTier(1, 49.99f), new PlanTier(2, 99.99f)));
            AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 300.00f, 399.99f, new PlanTier(1, 59.99f), new PlanTier(2, 139.99f)));
            AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 400.00f, 499.99f, new PlanTier(1, 79.99f), new PlanTier(2, 179.99f)));
            AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 500.00f, 749.99f, new PlanTier(1, 99.99f), new PlanTier(2, 199.99f)));
            AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 750.00f, 999.99f, new PlanTier(1, 199.99f), new PlanTier(2, 299.99f)));
            AllPlans.Add(new PlanReference(PlanType.Small_Electronic_ADH, 1000.00f, 1499.99f, new PlanTier(1, 299.99f), new PlanTier(2, 399.99f)));

            AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 0.00f, 49.99f, new PlanTier(2, 6.99f), new PlanTier(3, 9.99f)));
            AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 50.00f, 99.99f, new PlanTier(2, 14.99f), new PlanTier(3, 29.99f)));
            AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 100.00f, 199.99f, new PlanTier(2, 29.99f), new PlanTier(3, 49.99f)));
            AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 200.00f, 299.99f, new PlanTier(2, 49.99f), new PlanTier(3, 69.99f)));
            AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 300.00f, 399.99f, new PlanTier(2, 69.99f), new PlanTier(3, 99.99f)));
            AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 400.00f, 499.99f, new PlanTier(2, 89.99f), new PlanTier(3, 129.99f)));
            AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 500.00f, 999.99f, new PlanTier(2, 139.99f), new PlanTier(3, 199.99f)));
            AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 1000.00f, 1499.99f, new PlanTier(2, 199.99f), new PlanTier(3, 279.99f)));
            AllPlans.Add(new PlanReference(PlanType.BYO_Replacement, 1500.00f, 3000.00f, new PlanTier(2, 299.99f), new PlanTier(3, 429.99f)));


            AllPlans.Add(new PlanReference(PlanType.Build_Plan, 0.00f,     399.99f,   new PlanTier(2, 49.99f),   new PlanTier(3, 59.99f)));
            AllPlans.Add(new PlanReference(PlanType.Build_Plan, 400.00f,   599.99f,   new PlanTier(2, 69.99f),   new PlanTier(3, 99.99f)));
            AllPlans.Add(new PlanReference(PlanType.Build_Plan, 600.00f,   799.99f,   new PlanTier(2, 99.99f),   new PlanTier(3, 129.99f)));
            AllPlans.Add(new PlanReference(PlanType.Build_Plan, 800.00f,   999.99f,   new PlanTier(2, 119.99f),  new PlanTier(3, 149.99f)));
            AllPlans.Add(new PlanReference(PlanType.Build_Plan, 1000.00f,  1199.99f,  new PlanTier(2, 129.99f),  new PlanTier(3, 179.99f)));
            AllPlans.Add(new PlanReference(PlanType.Build_Plan, 1200.00f,  1499.99f,  new PlanTier(2, 149.99f),  new PlanTier(3, 199.99f)));
            AllPlans.Add(new PlanReference(PlanType.Build_Plan, 1500.00f,  1999.99f,  new PlanTier(2, 199.99f),  new PlanTier(3, 249.99f)));
            AllPlans.Add(new PlanReference(PlanType.Build_Plan, 2000.00f,  3999.99f,  new PlanTier(2, 249.99f),  new PlanTier(3, 279.99f)));
            AllPlans.Add(new PlanReference(PlanType.Build_Plan, 4000.00f,  5999.99f,  new PlanTier(2, 399.99f),  new PlanTier(3, 449.99f)));
            AllPlans.Add(new PlanReference(PlanType.Build_Plan, 6000.00f,  7499.99f,  new PlanTier(2, 549.99f),  new PlanTier(3, 699.99f)));
            AllPlans.Add(new PlanReference(PlanType.Build_Plan, 7500.00f,  9999.99f,  new PlanTier(2, 699.99f),  new PlanTier(3, 799.99f)));
            AllPlans.Add(new PlanReference(PlanType.Build_Plan, 10000.00f, 14999.99f, new PlanTier(2, 1299.99f), new PlanTier(3, 1499.99f)));
            AllPlans.Add(new PlanReference(PlanType.Build_Plan, 15000.00f, 20000.00f, new PlanTier(2, 1499.99f), new PlanTier(3, 1999.99f)));

        }
    }
}
