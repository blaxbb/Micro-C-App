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
            Apple_Plans_iPad,
            Apple_Plans_ADH_iPad,
            Apple_Plans_13,
            Apple_Plans_ADH_13,
            Apple_Plans_15_and_16,
            Apple_Plans_ADH_15_and_16,
            Apple_Plans_iMac,
            Apple_Plans_ADH_iMac,
            Apple_Plans_Mac_Pro,
            Apple_Plans_ADH_Mac_Pro,
            Apple_Plans_Mac_Mini,
            Apple_Plans_ADH_Mac_Mini,
            Laptop_ADH,
            Laptop_DOP,
            Laptop_Extension,
            Desktop_ADH,
            Desktop_DOP,
            Desktop_Extension,
            Tablet_ADH,
            Tablet_DOP,
            Tablet_Extension,
            Carry_In,
            AppleCare_13_MBP,
            AppleCare_13_MBA,
            AppleCare_15,
            AppleCare_Mac_Mini,
            AppleCare_iMac,
            AppleCare_Mac_Pro,
            AppleCare_Apple_TV,
            AppleCare_Display,
            AppleCare_iPod_Touch,
            AppleCare_iPad,
            AppleCare_iPad_Pro,
            AppleCare_iPhone,
            AppleCare_Watch_S3,
            AppleCare_Watch_S4_S5,
            AppleCare_Watch_Stainless,
            AppleCare_HomePod,
            AppleCare_Headphones,

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

            

            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 0.00f, 99.99f, new PlanTier(1, 19.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 100.00f, 199.99f, new PlanTier(1, 29.99f), new PlanTier(2, 79.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 200.00f, 299.99f, new PlanTier(1, 49.99f), new PlanTier(2, 119.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 300.00f, 399.99f, new PlanTier(1, 59.99f), new PlanTier(2, 129.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 400.00f, 499.99f, new PlanTier(1, 79.99f), new PlanTier(2, 149.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 500.00f, 599.99f, new PlanTier(1, 99.99f), new PlanTier(2, 179.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 600.00f, 699.99f, new PlanTier(1, 119.99f), new PlanTier(2, 199.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 700.00f, 799.99f, new PlanTier(1, 129.99f), new PlanTier(2, 239.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 800.00f, 899.99f, new PlanTier(1, 139.99f), new PlanTier(2, 269.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 900.00f, 999.99f, new PlanTier(1, 149.99f), new PlanTier(2, 299.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 1000.00f, 1099.99f, new PlanTier(1, 199.99f), new PlanTier(2, 329.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 1100.00f, 1199.99f, new PlanTier(1, 219.99f), new PlanTier(2, 339.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 1200.00f, 1299.99f, new PlanTier(1, 239.99f), new PlanTier(2, 359.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 1300.00f, 1399.99f, new PlanTier(1, 259.99f), new PlanTier(2, 379.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 1400.00f, 1499.99f, new PlanTier(1, 279.99f), new PlanTier(2, 399.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 1500.00f, 1999.99f, new PlanTier(1, 299.99f), new PlanTier(2, 449.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 2000.00f, 2999.99f, new PlanTier(1, 339.99f), new PlanTier(2, 499.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 3000.00f, 4999.99f, new PlanTier(1, 369.99f), new PlanTier(2, 549.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_ADH, 5000.00f, 5999.99f, new PlanTier(1, 399.99f), new PlanTier(2, 599.99f)));

            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 0.00f, 99.99f, new PlanTier(2, 19.99f), new PlanTier(3, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 100.00f, 199.99f, new PlanTier(2, 29.99f), new PlanTier(3, 79.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 200.00f, 299.99f, new PlanTier(2, 49.99f), new PlanTier(3, 119.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 300.00f, 399.99f, new PlanTier(2, 59.99f), new PlanTier(3, 129.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 400.00f, 499.99f, new PlanTier(2, 79.99f), new PlanTier(3, 149.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 500.00f, 599.99f, new PlanTier(2, 99.99f), new PlanTier(3, 179.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 600.00f, 699.99f, new PlanTier(2, 119.99f), new PlanTier(3, 199.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 700.00f, 799.99f, new PlanTier(2, 129.99f), new PlanTier(3, 239.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 800.00f, 899.99f, new PlanTier(2, 139.99f), new PlanTier(3, 269.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 900.00f, 999.99f, new PlanTier(2, 149.99f), new PlanTier(3, 299.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 1000.00f, 1099.99f, new PlanTier(2, 199.99f), new PlanTier(3, 329.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 1100.00f, 1199.99f, new PlanTier(2, 219.99f), new PlanTier(3, 334.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 1200.00f, 1299.99f, new PlanTier(2, 239.99f), new PlanTier(3, 359.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 1300.00f, 1399.99f, new PlanTier(2, 259.99f), new PlanTier(3, 379.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 1400.00f, 1499.99f, new PlanTier(2, 279.99f), new PlanTier(3, 399.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 1500.00f, 1999.99f, new PlanTier(2, 299.99f), new PlanTier(3, 449.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 2000.00f, 2999.99f, new PlanTier(2, 339.99f), new PlanTier(3, 499.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_DOP, 3000.00f, 4999.99f, new PlanTier(2, 369.99f), new PlanTier(3, 549.99f)));

            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 0.00f, 99.99f, new PlanTier(1, 19.99f), new PlanTier(2, 29.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 100.00f, 199.99f, new PlanTier(1, 29.99f), new PlanTier(2, 59.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 200.00f, 299.99f, new PlanTier(1, 39.99f), new PlanTier(2, 79.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 300.00f, 399.99f, new PlanTier(1, 49.99f), new PlanTier(2, 89.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 400.00f, 499.99f, new PlanTier(1, 59.99f), new PlanTier(2, 99.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 500.00f, 599.99f, new PlanTier(1, 69.99f), new PlanTier(2, 119.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 600.00f, 699.99f, new PlanTier(1, 79.99f), new PlanTier(2, 129.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 700.00f, 799.99f, new PlanTier(1, 89.99f), new PlanTier(2, 159.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 800.00f, 899.99f, new PlanTier(1, 99.99f), new PlanTier(2, 179.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 900.00f, 999.99f, new PlanTier(1, 109.99f), new PlanTier(2, 199.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 1000.00f, 1099.99f, new PlanTier(1, 139.99f), new PlanTier(2, 249.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 1100.00f, 1199.99f, new PlanTier(1, 149.99f), new PlanTier(2, 259.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 1200.00f, 1299.99f, new PlanTier(1, 159.99f), new PlanTier(2, 269.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 1300.00f, 1399.99f, new PlanTier(1, 169.99f), new PlanTier(2, 279.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 1400.00f, 1499.99f, new PlanTier(1, 179.99f), new PlanTier(2, 289.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 1500.00f, 1999.99f, new PlanTier(1, 199.99f), new PlanTier(2, 299.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 2000.00f, 2999.99f, new PlanTier(1, 199.99f), new PlanTier(2, 299.99f)));
            AllPlans.Add(new PlanReference(PlanType.Laptop_Extension, 3000.00f, 4999.99f, new PlanTier(1, 199.99f), new PlanTier(2, 299.99f)));

            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 0.00f, 99.99f,  new PlanTier(2, 79.99f), new PlanTier(3, 0)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 100.00f, 199.99f, new PlanTier(2, 79.99f), new PlanTier(3, 0)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 200.00f, 299.99f, new PlanTier(2, 89.99f), new PlanTier(3, 139.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 300.00f, 399.99f, new PlanTier(2, 99.99f), new PlanTier(3, 149.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 400.00f, 499.99f, new PlanTier(2, 139.99f), new PlanTier(3, 169.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 500.00f, 599.99f, new PlanTier(2, 149.99f), new PlanTier(3, 179.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 600.00f, 699.99f, new PlanTier(2, 159.99f), new PlanTier(3, 199.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 700.00f, 799.99f, new PlanTier(2, 179.99f), new PlanTier(3, 219.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 800.00f, 899.99f, new PlanTier(2, 189.99f), new PlanTier(3, 229.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 900.00f, 999.99f, new PlanTier(2, 199.99f), new PlanTier(3, 239.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 1000.00f, 1099.99f, new PlanTier(2, 209.99f), new PlanTier(3, 249.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 1100.00f, 1199.99f, new PlanTier(2, 219.99f), new PlanTier(3, 259.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 1200.00f, 1299.99f, new PlanTier(2, 229.99f), new PlanTier(3, 269.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 1300.00f, 1399.99f, new PlanTier(2, 239.99f), new PlanTier(3, 279.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 1400.00f, 1499.99f, new PlanTier(2, 239.99f), new PlanTier(3, 289.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 1500.00f, 1599.99f, new PlanTier(2, 249.99f), new PlanTier(3, 299.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 1600.00f, 1999.99f, new PlanTier(2, 269.99f), new PlanTier(3, 329.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 2000.00f, 2999.99f, new PlanTier(2, 329.99f), new PlanTier(3, 379.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 3000.00f, 4999.99f, new PlanTier(2, 349.99f), new PlanTier(3, 399.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_ADH, 5000.00f, 5999.99f, new PlanTier(2, 389.99f), new PlanTier(3, 449.99f)));

            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 0.00f, 99.99f, new PlanTier(2, 49.99f), new PlanTier(3, 59.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 100.00f, 199.99f, new PlanTier(2, 49.99f), new PlanTier(3, 59.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 200.00f, 299.99f, new PlanTier(2, 59.99f), new PlanTier(3, 79.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 300.00f, 399.99f, new PlanTier(2, 69.99f), new PlanTier(3, 89.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 400.00f, 499.99f, new PlanTier(2, 89.99f), new PlanTier(3, 129.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 500.00f, 599.99f, new PlanTier(2, 109.99f), new PlanTier(3, 139.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 600.00f, 699.99f, new PlanTier(2, 119.99f), new PlanTier(3, 149.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 700.00f, 799.99f, new PlanTier(2, 129.99f), new PlanTier(3, 169.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 800.00f, 899.99f, new PlanTier(2, 139.99f), new PlanTier(3, 179.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 900.00f, 999.99f, new PlanTier(2, 159.99f), new PlanTier(3, 189.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 1000.00f, 1099.99f, new PlanTier(2, 179.99f), new PlanTier(3, 209.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 1100.00f, 1199.99f, new PlanTier(2, 189.99f), new PlanTier(3, 219.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 1200.00f, 1299.99f, new PlanTier(2, 189.99f), new PlanTier(3, 239.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 1300.00f, 1399.99f, new PlanTier(2, 199.99f), new PlanTier(3, 239.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 1400.00f, 1499.99f, new PlanTier(2, 209.99f), new PlanTier(3, 239.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 1500.00f, 1599.99f, new PlanTier(2, 229.99f), new PlanTier(3, 259.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 1600.00f, 1999.99f, new PlanTier(2, 249.99f), new PlanTier(3, 279.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 2000.00f, 2999.99f, new PlanTier(2, 269.99f), new PlanTier(3, 299.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 3000.00f, 4999.99f, new PlanTier(2, 309.99f), new PlanTier(3, 349.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_DOP, 5000.00f, 5999.99f, new PlanTier(2, 329.99f), new PlanTier(3, 379.99f)));

            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 0.00f, 99.99f, new PlanTier(1, 19.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 100.00f, 199.99f, new PlanTier(1, 29.99f), new PlanTier(2, 49.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 200.00f, 299.99f, new PlanTier(1, 39.99f), new PlanTier(2, 59.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 300.00f, 399.99f, new PlanTier(1, 59.99f), new PlanTier(2, 69.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 400.00f, 499.99f, new PlanTier(1, 69.99f), new PlanTier(2, 89.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 500.00f, 599.99f, new PlanTier(1, 79.99f), new PlanTier(2, 109.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 600.00f, 699.99f, new PlanTier(1, 99.99f), new PlanTier(2, 119.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 700.00f, 799.99f, new PlanTier(1, 109.99f), new PlanTier(2, 139.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 800.00f, 899.99f, new PlanTier(1, 119.99f), new PlanTier(2, 149.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 900.00f, 999.99f, new PlanTier(1, 139.99f), new PlanTier(2, 169.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 1000.00f, 1099.99f, new PlanTier(1, 149.99f), new PlanTier(2, 189.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 1100.00f, 1199.99f, new PlanTier(1, 159.99f), new PlanTier(2, 199.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 1200.00f, 1299.99f, new PlanTier(1, 169.99f), new PlanTier(2, 199.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 1300.00f, 1399.99f, new PlanTier(1, 179.99f), new PlanTier(2, 209.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 1400.00f, 1499.99f, new PlanTier(1, 189.99f), new PlanTier(2, 219.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 1500.00f, 1599.99f, new PlanTier(1, 199.99f), new PlanTier(2, 239.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 1600.00f, 1999.99f, new PlanTier(1, 219.99f), new PlanTier(2, 259.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 2000.00f, 2999.99f, new PlanTier(1, 239.99f), new PlanTier(2, 279.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 3000.00f, 4999.99f, new PlanTier(1, 269.99f), new PlanTier(2, 299.99f)));
            AllPlans.Add(new PlanReference(PlanType.Desktop_Extension, 5000.00f, 5999.99f, new PlanTier(1, 279.99f), new PlanTier(2, 309.99f)));

            AllPlans.Add(new PlanReference(PlanType.Apple_Plans_ADH_13, 0.00f, 0.00f, new PlanTier(3, 319.99f), new PlanTier(2, 239.99f)));
            AllPlans.Add(new PlanReference(PlanType.Apple_Plans_13, 0.00f, 0.00f, new PlanTier(3, 149.99f)));
            AllPlans.Add(new PlanReference(PlanType.Apple_Plans_ADH_15_and_16, 0.00f, 0.00f, new PlanTier(3, 429.99f), new PlanTier(2, 349.99f)));
            AllPlans.Add(new PlanReference(PlanType.Apple_Plans_15_and_16, 0.00f, 0.00f, new PlanTier(3, 149.99f)));AllPlans.Add(new PlanReference(PlanType.Apple_Plans_ADH_Mac_Mini, 0.00f, 0.00f, new PlanTier(2, 119.99f), new PlanTier(3, 149.99f)));
            AllPlans.Add(new PlanReference(PlanType.Apple_Plans_Mac_Mini, 0.00f, 0.00f, new PlanTier(3, 74.99f)));
            AllPlans.Add(new PlanReference(PlanType.Apple_Plans_ADH_Mac_Pro, 0.00f, 0.00f, new PlanTier(3, 299.99f), new PlanTier(2, 219.99f)));
            AllPlans.Add(new PlanReference(PlanType.Apple_Plans_Mac_Pro, 0.00f, 0.00f, new PlanTier(3, 149.99f)));
            AllPlans.Add(new PlanReference(PlanType.Apple_Plans_ADH_iMac, 0.00f, 0.00f, new PlanTier(3, 219.99f), new PlanTier(2, 169.99f)));
            AllPlans.Add(new PlanReference(PlanType.Apple_Plans_iMac, 0.00f, 0.00f, new PlanTier(3, 149.99f)));
            AllPlans.Add(new PlanReference(PlanType.Apple_Plans_ADH_iPad, 0.00f, 299.99f, new PlanTier(2, 69.99f), new PlanTier(1, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Apple_Plans_ADH_iPad, 300.00f, 1699.99f, new PlanTier(2, 99.99f), new PlanTier(1, 69.99f)));
            AllPlans.Add(new PlanReference(PlanType.Apple_Plans_iPad, 0.00f, 299.99f, new PlanTier(3, 59.99f)));
            AllPlans.Add(new PlanReference(PlanType.Apple_Plans_iPad, 300.00f, 1699.99f, new PlanTier(3, 79.99f)));

            AllPlans.Add(new PlanReference(PlanType.Tablet_ADH, 0.00f, 99.99f, new PlanTier(1, 24.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_ADH, 100.00f, 199.99f, new PlanTier(1, 39.99f), new PlanTier(2, 79.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_ADH, 200.00f, 299.99f, new PlanTier(1, 49.99f), new PlanTier(2, 119.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_ADH, 300.00f, 399.99f, new PlanTier(1, 59.99f), new PlanTier(2, 129.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_ADH, 400.00f, 499.99f, new PlanTier(1, 79.99f), new PlanTier(2, 149.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_ADH, 500.00f, 599.99f, new PlanTier(1, 99.99f), new PlanTier(2, 179.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_ADH, 600.00f, 699.99f, new PlanTier(1, 119.99f), new PlanTier(2, 199.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_ADH, 700.00f, 799.99f, new PlanTier(1, 129.99f), new PlanTier(2, 239.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_ADH, 800.00f, 899.99f, new PlanTier(1, 139.99f), new PlanTier(2, 269.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_ADH, 900.00f, 999.99f, new PlanTier(1, 149.99f), new PlanTier(2, 299.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_ADH, 1000.00f, 1499.99f, new PlanTier(1, 199.99f), new PlanTier(2, 329.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_ADH, 1500.00f, 5000.00f, new PlanTier(1, 199.99f), new PlanTier(2, 329.99f)));

            AllPlans.Add(new PlanReference(PlanType.Tablet_DOP, 0.00f, 99.99f, new PlanTier(2, 24.99f), new PlanTier(3, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_DOP, 100.00f, 199.99f, new PlanTier(2, 39.99f), new PlanTier(3, 79.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_DOP, 200.00f, 299.99f, new PlanTier(2, 49.99f), new PlanTier(3, 119.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_DOP, 300.00f, 399.99f, new PlanTier(2, 59.99f), new PlanTier(3, 129.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_DOP, 400.00f, 499.99f, new PlanTier(2, 79.99f), new PlanTier(3, 149.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_DOP, 500.00f, 599.99f, new PlanTier(2, 99.99f), new PlanTier(3, 179.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_DOP, 600.00f, 699.99f, new PlanTier(2, 119.99f), new PlanTier(3, 199.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_DOP, 700.00f, 799.99f, new PlanTier(2, 129.99f), new PlanTier(3, 239.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_DOP, 800.00f, 899.99f, new PlanTier(2, 139.99f), new PlanTier(3, 269.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_DOP, 900.00f, 999.99f, new PlanTier(2, 149.99f), new PlanTier(3, 299.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_DOP, 1000.00f, 1499.99f, new PlanTier(2, 199.99f), new PlanTier(3, 329.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_DOP, 1500.00f, 5000.00f, new PlanTier(2, 199.99f), new PlanTier(3, 329.99f)));

            AllPlans.Add(new PlanReference(PlanType.Tablet_Extension, 0.00f, 99.99f, new PlanTier(1, 24.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_Extension, 100.00f, 199.99f, new PlanTier(1, 24.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_Extension, 200.00f, 299.99f, new PlanTier(1, 24.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_Extension, 300.00f, 399.99f, new PlanTier(1, 24.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_Extension, 400.00f, 499.99f, new PlanTier(1, 24.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_Extension, 500.00f, 599.99f, new PlanTier(1, 24.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_Extension, 600.00f, 699.99f, new PlanTier(1, 24.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_Extension, 700.00f, 799.99f, new PlanTier(1, 24.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_Extension, 800.00f, 899.99f, new PlanTier(1, 24.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_Extension, 900.00f, 999.99f, new PlanTier(1, 24.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_Extension, 1000.00f, 1499.99f, new PlanTier(1, 24.99f), new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.Tablet_Extension, 1500.00f, 5000.00f, new PlanTier(1, 24.99f), new PlanTier(2, 39.99f)));

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

            AllPlans.Add(new PlanReference(PlanType.Carry_In, 0.00f, 99.99f, new PlanTier(2, 19.99f), new PlanTier(3, 29.99f)));
            AllPlans.Add(new PlanReference(PlanType.Carry_In, 100.00f, 199.99f, new PlanTier(2, 29.99f), new PlanTier(3, 59.99f)));
            AllPlans.Add(new PlanReference(PlanType.Carry_In, 200.00f, 299.99f, new PlanTier(2, 39.99f), new PlanTier(3, 79.99f)));
            AllPlans.Add(new PlanReference(PlanType.Carry_In, 300.00f, 399.99f, new PlanTier(2, 49.99f), new PlanTier(3, 89.99f)));
            AllPlans.Add(new PlanReference(PlanType.Carry_In, 400.00f, 499.99f, new PlanTier(2, 59.99f), new PlanTier(3, 99.99f)));
            AllPlans.Add(new PlanReference(PlanType.Carry_In, 500.00f, 599.99f, new PlanTier(2, 69.99f), new PlanTier(3, 119.99f)));
            AllPlans.Add(new PlanReference(PlanType.Carry_In, 600.00f, 699.99f, new PlanTier(2, 79.99f), new PlanTier(3, 129.99f)));
            AllPlans.Add(new PlanReference(PlanType.Carry_In, 700.00f, 799.99f, new PlanTier(2, 89.99f), new PlanTier(3, 159.99f)));
            AllPlans.Add(new PlanReference(PlanType.Carry_In, 800.00f, 899.99f, new PlanTier(2, 99.99f), new PlanTier(3, 179.99f)));
            AllPlans.Add(new PlanReference(PlanType.Carry_In, 900.00f, 999.99f, new PlanTier(2, 109.99f), new PlanTier(3, 199.99f)));
            AllPlans.Add(new PlanReference(PlanType.Carry_In, 1000.00f, 1499.99f, new PlanTier(2, 139.99f), new PlanTier(3, 249.99f)));
            AllPlans.Add(new PlanReference(PlanType.Carry_In, 1500.00f, 1999.99f, new PlanTier(2, 149.99f), new PlanTier(3, 249.99f)));
            AllPlans.Add(new PlanReference(PlanType.Carry_In, 2000.00f, 2999.99f, new PlanTier(2, 199.99f), new PlanTier(3, 299.99f)));
            AllPlans.Add(new PlanReference(PlanType.Carry_In, 3000.00f, 4999.99f, new PlanTier(2, 249.99f), new PlanTier(3, 349.99f)));

            AllPlans.Add(new PlanReference(PlanType.AppleCare_13_MBA, 0f, 0f, new PlanTier(3, 249.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_13_MBP, 0f, 0f, new PlanTier(3, 269.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_15, 0f, 0f, new PlanTier(3, 379.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_Mac_Mini, 0f, 0f, new PlanTier(3, 99.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_iMac, 0f, 0f, new PlanTier(3, 169.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_Mac_Pro, 0f, 0f, new PlanTier(3, 299.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_Watch_S3, 0f, 0f, new PlanTier(2, 49.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_Watch_S4_S5, 0f, 0f, new PlanTier(2, 79.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_Watch_Stainless, 0f, 0f, new PlanTier(2, 149.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_Apple_TV, 0f, 0f, new PlanTier(2, 29.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_Display, 0f, 0f, new PlanTier(3, 99.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_iPod_Touch, 0f, 0f, new PlanTier(2, 59.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_iPad, 0f, 0f, new PlanTier(2, 69.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_iPhone, 0f, 0f, new PlanTier(2, 99.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_iPad_Pro, 0f, 0f, new PlanTier(2, 129.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_HomePod, 0f, 0f, new PlanTier(2, 39.99f)));
            AllPlans.Add(new PlanReference(PlanType.AppleCare_Headphones, 0f, 0f, new PlanTier(2, 29.99f)));
        }
    }
}
