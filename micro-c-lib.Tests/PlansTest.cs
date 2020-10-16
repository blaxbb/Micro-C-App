using MicroCLib.Models;
using MicroCLib.Models.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static MicroCLib.Models.Reference.PlanReference;

namespace micro_c_lib.Tests
{
    [TestClass]
    public class PlansTest
    {
        [DataTestMethod]
        public void PlansExist()
        {
            Assert.IsNotNull(PlanReference.AllPlans);
            Assert.IsTrue(PlanReference.AllPlans.Count > 0);
        }

        [TestMethod]
        public void AllTypesHavePlans()
        {
            var planTypes = Enum.GetValues(typeof(PlanType));
            foreach (PlanType type in planTypes)
            {
                var plans = PlanReference.AllPlans.Where(p => p.Type == type);
                Assert.IsTrue(plans.Count() > 0, type.ToString());
            }
        }

        [TestMethod]
        public void PriceTierInTierSanity()
        {
            //checks to make sure higher duration plan costs more
            foreach(var plan in PlanReference.AllPlans)
            {
                float checkPrice = 0f;
                foreach (var tier in plan.Tiers.OrderBy(t => t.Duration))
                {
                    Assert.IsTrue(tier.Price > checkPrice, $"{plan.Type} => {plan.MinPrice} - {plan.MaxPrice} | {tier.Duration}yr");
                    checkPrice = tier.Price;
                }
            }
        }

        [TestMethod]
        public void DurationPriceSanity()
        {
            //checks to see that there are no gaps between plan coverages
            var planTypes = Enum.GetValues(typeof(PlanType));
            foreach(PlanType type in planTypes)
            {
                var plans = PlanReference.AllPlans.Where(p => p.Type == type).OrderBy(p => p.MinPrice).ToList();
                //starts first plan at $0.00
                var lastHigh = -.01f;
                foreach(var plan in plans)
                {
                    Assert.IsTrue(plan.MinPrice == lastHigh + .01f, $"{type} => {plan.MinPrice} does not equal previous max price of {lastHigh}");
                    lastHigh = plan.MaxPrice;
                }
            }
        }
    }
}
