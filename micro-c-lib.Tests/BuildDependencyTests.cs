using MicroCLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using static MicroCLib.Models.BuildComponent;

namespace micro_c_lib.Tests
{
    [TestClass]
    public class BuildDependencyTests
    {
        [TestMethod]
        public void FieldTestMatches()
        {
            var dep = new FieldContainsDependency(ComponentType.CPU, "Socket Type", ComponentType.Motherboard, "Socket Type");
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "R5 3600",
                    ComponentType = ComponentType.CPU,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Socket Type", "AM4" }
                    }
                },
                new Item()
                {
                    Name = "x570 Board",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Socket Type", "AM4" }
                    }
                }
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 0);
        }

        [TestMethod]
        public void FieldTestDoesntMatch()
        {
            var dep = new FieldContainsDependency(ComponentType.CPU, "Socket Type", ComponentType.Motherboard, "Socket Type");
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "R5 3600",
                    ComponentType = ComponentType.CPU,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Socket Type", "AM4" }
                    }
                },
                new Item()
                {
                    Name = "z490 Board",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Socket Type", "LGA 1200" }
                    }
                }
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 2);
        }

        [TestMethod]
        public void FieldTestMissing()
        {
            var dep = new FieldContainsDependency(ComponentType.CPU, "Socket Type", ComponentType.Motherboard, "Socket Type");
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "R5 3600",
                    ComponentType = ComponentType.CPU,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Socket Type", "AM4" }
                    }
                },
                new Item()
                {
                    Name = "Unknown Board",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                    }
                }
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 0);

            dep = new FieldContainsDependency(ComponentType.Motherboard, "Socket Type", ComponentType.CPU, "Socket Type");

            Assert.AreEqual(dep.HasErrors(items).Count, 0);
        }

        [TestMethod]
        public void FieldTestQuantityNormal()
        {
            var dep = new FieldQuantityDependency(ComponentType.HDD, ComponentType.Case, "Internal 3.5\" Drive Bays");
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "3.5 HDD",
                    ComponentType = ComponentType.HDD,
                },
                new Item()
                {
                    Name = "3.5 HDD",
                    ComponentType = ComponentType.HDD,
                },
                new Item()
                {
                    Name = "Unknown Case",
                    ComponentType = ComponentType.Case,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Internal 3.5\" Drive Bays", "2" }
                    }
                }
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 0);
        }

        [TestMethod]
        public void FieldTestQuantityNone()
        {
            var dep = new FieldQuantityDependency(ComponentType.HDD, ComponentType.Case, "Internal 3.5\" Drive Bays");
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "Unknown Case",
                    ComponentType = ComponentType.Case,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Internal 3.5\" Drive Bays", "2" }
                    }
                }
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 0);
        }

        [TestMethod]
        public void FieldTestQuantityMissingField()
        {
            var dep = new FieldQuantityDependency(ComponentType.HDD, ComponentType.Case, "Internal 3.5\" Drive Bays");
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "3.5 HDD",
                    ComponentType = ComponentType.HDD,
                },
                new Item()
                {
                    Name = "3.5 HDD",
                    ComponentType = ComponentType.HDD,
                },
                new Item()
                {
                    Name = "Unknown Case",
                    ComponentType = ComponentType.Case,
                    Specs = new Dictionary<string, string>()
                    {
                    }
                }
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 3);
        }

        [TestMethod]
        public void FieldTestQuantityTooMany()
        {
            var dep = new FieldQuantityDependency(ComponentType.HDD, ComponentType.Case, "Internal 3.5\" Drive Bays");
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "3.5 HDD",
                    ComponentType = ComponentType.HDD,
                },
                new Item()
                {
                    Name = "3.5 HDD",
                    ComponentType = ComponentType.HDD,
                },
                new Item()
                {
                    Name = "3.5 HDD",
                    ComponentType = ComponentType.HDD,
                },
                new Item()
                {
                    Name = "Unknown Case",
                    ComponentType = ComponentType.Case,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Internal 3.5\" Drive Bays", "2" }
                    }
                }
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 4);
        }

        [TestMethod]
        public void FieldTestComparisonCompare()
        {
            Assert.IsTrue(FieldComparisonDependency.Compare(2, 5, FieldComparisonDependency.CompareMode.LessThan));
            Assert.IsTrue(FieldComparisonDependency.Compare(2, 5, FieldComparisonDependency.CompareMode.LessThanOrEqual));
            Assert.IsTrue(FieldComparisonDependency.Compare(5, 5, FieldComparisonDependency.CompareMode.LessThanOrEqual));

            Assert.IsFalse(FieldComparisonDependency.Compare(5, 2, FieldComparisonDependency.CompareMode.LessThan));
            Assert.IsFalse(FieldComparisonDependency.Compare(5, 2, FieldComparisonDependency.CompareMode.LessThanOrEqual));

            Assert.IsTrue(FieldComparisonDependency.Compare(5, 2, FieldComparisonDependency.CompareMode.GreaterThan));
            Assert.IsTrue(FieldComparisonDependency.Compare(5, 2, FieldComparisonDependency.CompareMode.GreaterThanOrEqual));
            Assert.IsTrue(FieldComparisonDependency.Compare(5, 5, FieldComparisonDependency.CompareMode.GreaterThanOrEqual));

            Assert.IsFalse(FieldComparisonDependency.Compare(2, 5, FieldComparisonDependency.CompareMode.GreaterThan));
            Assert.IsFalse(FieldComparisonDependency.Compare(2, 5, FieldComparisonDependency.CompareMode.GreaterThanOrEqual));

            Assert.IsTrue(FieldComparisonDependency.Compare(5, 5, FieldComparisonDependency.CompareMode.Equal));
            Assert.IsFalse(FieldComparisonDependency.Compare(2, 5, FieldComparisonDependency.CompareMode.Equal));
        }

        [TestMethod]
        public void FieldTestComparisonNormal()
        {
            var dep = new FieldComparisonDependency(ComponentType.GPU, "Recommended Power Supply", ComponentType.PowerSupply, "Wattage", FieldComparisonDependency.CompareMode.LessThanOrEqual);
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "GPU",
                    ComponentType = ComponentType.GPU,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Recommended Power Supply", "500 Watts" }
                    }
                },
                new Item()
                {
                    Name = "PSU",
                    ComponentType = ComponentType.PowerSupply,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Wattage", "750 Watts" }
                    }
                },
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 0);
        }

        [TestMethod]
        public void FieldTestComparisonFailure()
        {
            var dep = new FieldComparisonDependency(ComponentType.GPU, "Recommended Power Supply", ComponentType.PowerSupply, "Wattage", FieldComparisonDependency.CompareMode.LessThanOrEqual);
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "GPU",
                    ComponentType = ComponentType.GPU,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Recommended Power Supply", "500 Watts" }
                    }
                },
                new Item()
                {
                    Name = "PSU",
                    ComponentType = ComponentType.PowerSupply,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Wattage", "100 Watts" }
                    }
                },
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 2);
        }

        [TestMethod]
        public void FieldTestComparisonMissingField()
        {
            var dep = new FieldComparisonDependency(ComponentType.GPU, "Recommended Power Supply", ComponentType.PowerSupply, "Wattage", FieldComparisonDependency.CompareMode.LessThanOrEqual)
            {
                FailOnEmpty = true
            };
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "GPU",
                    ComponentType = ComponentType.GPU,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Recommended Power Supply", "500 Watts" }
                    }
                },
                new Item()
                {
                    Name = "PSU",
                    ComponentType = ComponentType.PowerSupply,
                    Specs = new Dictionary<string, string>()
                    {
                    }
                },
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 2);

            items = new List<Item>()
            {
                new Item()
                {
                    Name = "GPU",
                    ComponentType = ComponentType.GPU,
                    Specs = new Dictionary<string, string>()
                    {
                    }
                },
                new Item()
                {
                    Name = "PSU",
                    ComponentType = ComponentType.PowerSupply,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Wattage", "100 Watts" }
                    }
                },
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 2);
        }
    }
}
