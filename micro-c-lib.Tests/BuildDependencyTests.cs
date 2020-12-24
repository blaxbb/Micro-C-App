using micro_c_lib.Models.Build;
using MicroCLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static MicroCLib.Models.BuildComponent;

namespace micro_c_lib.Tests
{
    [TestClass]
    public class BuildDependencyTests
    {
        [TestMethod]
        [TestCategory("FieldContainsDependency")]
        public void FieldTestMatches()
        {
            var dep = new FieldContainsDependency("", ComponentType.CPU, "Socket Type", ComponentType.Motherboard, "Socket Type");
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
        [TestCategory("FieldContainsDependency")]
        public void FieldTestDoesntMatch()
        {
            var dep = new FieldContainsDependency("", ComponentType.CPU, "Socket Type", ComponentType.Motherboard, "Socket Type");
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
        [TestCategory("FieldContainsDependency")]
        public void FieldTestMissing()
        {
            var dep = new FieldContainsDependency("", ComponentType.CPU, "Socket Type", ComponentType.Motherboard, "Socket Type");
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

            dep = new FieldContainsDependency("", ComponentType.Motherboard, "Socket Type", ComponentType.CPU, "Socket Type");

            Assert.AreEqual(dep.HasErrors(items).Count, 0);
        }

        [TestMethod]
        [TestCategory("MemorySpeedDependency")]
        public void MemorySpeedTestMatches()
        {
            var dep = new MemorySpeedDependency("Motherboard Memory Speed", ComponentType.Motherboard, "Memory Speeds Supported", ComponentType.RAM, "Memory Speed (MHz)");
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "Mobo",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Memory Speeds Supported", "4400(O.C)/3466(O.C.)/3400(O.C.)/3200(O.C.)/3000(O.C.)/2933(O.C.)/2800(O.C.)/2666/2400/2133" }
                    }
                },
                new Item()
                {
                    Name = "RAM",
                    ComponentType = ComponentType.RAM,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Memory Speed (MHz)", "DDR4-3200" }
                    }
                }
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 0);
        }

        [TestMethod]
        [TestCategory("MemorySpeedDependency")]
        public void MemorySpeedDoesntMatch()
        {
            var dep = new MemorySpeedDependency("Motherboard Memory Speed", ComponentType.Motherboard, "Memory Speeds Supported", ComponentType.RAM, "Memory Speed (MHz)");
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "Mobo",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Memory Speeds Supported", "4400(O.C)/3466(O.C.)/3400(O.C.)/3200(O.C.)/3000(O.C.)/2933(O.C.)/2800(O.C.)/2666/2400/2133" }
                    }
                },
                new Item()
                {
                    Name = "RAM",
                    ComponentType = ComponentType.RAM,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Memory Speed (MHz)", "DDR4-5100" }
                    }
                }
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 2);
        }

        [TestMethod]
        [TestCategory("MemorySpeedDependency")]
        public void MemorySpeedMissing()
        {
            var dep = new MemorySpeedDependency("Motherboard Memory Speed", ComponentType.Motherboard, "Memory Speeds Supported", ComponentType.RAM, "Memory Speed (MHz)");
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "Mobo",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Memory Speeds Supported", "4400(O.C)/3466(O.C.)/3400(O.C.)/3200(O.C.)/3000(O.C.)/2933(O.C.)/2800(O.C.)/2666/2400/2133" }
                    }
                },
                new Item()
                {
                    Name = "RAM",
                    ComponentType = ComponentType.RAM,
                    Specs = new Dictionary<string, string>()
                    {
                    }
                }
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 0);

            dep = new MemorySpeedDependency("Motherboard Memory Speed", ComponentType.RAM, "Memory Speed (MHz)", ComponentType.Motherboard, "Memory Speeds Supported");

            Assert.AreEqual(dep.HasErrors(items).Count, 0);
        }

        [TestMethod]
        [TestCategory("SSDFormFactorDependency")]
        public void SSDFormFactorTestMatches()
        {
            var dep = new SSDFormFactorDependency("SSD Form Factor", ComponentType.Motherboard, "M.2 Port Type", ComponentType.SSD, "Form Factor");
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "Mobo",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        { "M.2 Port Type", "2242\n2260\n2280\n22110" }
                    }
                },
                new Item()
                {
                    Name = "SSD",
                    ComponentType = ComponentType.SSD,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Form Factor", "M.2 2280 M Key" }
                    }
                },
                new Item()
                {
                    Name = "SSD",
                    ComponentType = ComponentType.SSD,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Form Factor", "2.5\"" }
                    }
                }
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 0);
        }

        [TestMethod]
        [TestCategory("SSDFormFactorDependency")]
        public void SSDFormFactorDoesntMatch()
        {
            var dep = new SSDFormFactorDependency("SSD Form Factor", ComponentType.Motherboard, "M.2 Port Type", ComponentType.SSD, "Form Factor");
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "Mobo",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        { "M.2 Port Type", "2242\n2260\n22110" }
                    }
                },
                new Item()
                {
                    Name = "SSD",
                    ComponentType = ComponentType.SSD,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Form Factor", "M.2 2280 M Key" }
                    }
                },
                new Item()
                {
                    Name = "SSD",
                    ComponentType = ComponentType.SSD,
                    Specs = new Dictionary<string, string>()
                    {
                        { "Form Factor", "2.5\"" }
                    }
                }
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 2);
        }

        [TestMethod]
        [TestCategory("SSDFormFactorDependency")]
        public void SSDFormFactorMissing()
        {
            var dep = new SSDFormFactorDependency("SSD Form Factor", ComponentType.Motherboard, "M.2 Port Type", ComponentType.SSD, "Form Factor");
            var items = new List<Item>()
            {
                new Item()
                {
                    Name = "Mobo",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        { "M.2 Port Type", "2242\n2260\n2280\n22110" }
                    }
                },
                new Item()
                {
                    Name = "SSD",
                    ComponentType = ComponentType.SSD,
                    Specs = new Dictionary<string, string>()
                    {
                    }
                },
                new Item()
                {
                    Name = "SSD",
                    ComponentType = ComponentType.SSD,
                    Specs = new Dictionary<string, string>()
                    {
                    }
                }
            };

            Assert.AreEqual(dep.HasErrors(items).Count, 0);
        }


        [TestMethod]
        [TestCategory("FieldQuantityDependency")]
        public void FieldTestQuantityNormal()
        {
            var dep = new FieldQuantityDependency("", ComponentType.HDD, ComponentType.Case, "Internal 3.5\" Drive Bays");
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
        [TestCategory("FieldQuantityDependency")]
        public void FieldTestQuantityNone()
        {
            var dep = new FieldQuantityDependency("", ComponentType.HDD, ComponentType.Case, "Internal 3.5\" Drive Bays");
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
        [TestCategory("FieldQuantityDependency")]
        public void FieldTestQuantityMissingField()
        {
            var dep = new FieldQuantityDependency("", ComponentType.HDD, ComponentType.Case, "Internal 3.5\" Drive Bays");
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

            Assert.AreEqual(dep.HasErrors(items).Count, 0);
        }

        [TestMethod]
        [TestCategory("FieldQuantityDependency")]
        public void FieldTestQuantityTooMany()
        {
            var dep = new FieldQuantityDependency("", ComponentType.HDD, ComponentType.Case, "Internal 3.5\" Drive Bays");
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
        [TestCategory("FieldComparisonDependency")]
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
        [TestCategory("FieldComparisonDependency")]
        public void FieldTestComparisonNormal()
        {
            var dep = new FieldComparisonDependency("", ComponentType.GPU, "Recommended Power Supply", ComponentType.PowerSupply, "Wattage", FieldComparisonDependency.CompareMode.LessThanOrEqual);
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
        [TestCategory("FieldComparisonDependency")]
        public void FieldTestComparisonFailure()
        {
            var dep = new FieldComparisonDependency("", ComponentType.GPU, "Recommended Power Supply", ComponentType.PowerSupply, "Wattage", FieldComparisonDependency.CompareMode.LessThanOrEqual);
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
        [TestCategory("FieldComparisonDependency")]
        public void FieldTestComparisonMissingField()
        {
            var dep = new FieldComparisonDependency("", ComponentType.GPU, "Recommended Power Supply", ComponentType.PowerSupply, "Wattage", FieldComparisonDependency.CompareMode.LessThanOrEqual)
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

        private BuildComponentDependency Get(string name)
        {
            return BuildComponentDependency.Dependencies.FirstOrDefault(d => d.Name == name);
        }

        private void TestPermutations(BuildComponentDependency dep, Item a, Item b, string invalidSpec = "9999999")
        {
            //Order of permutations
            //
            //given params     -> No Error
            //b incorrect spec -> Error
            //a empty          -> No Error
            //b empty          -> No Error

            var items =  new List<Item>() { a, b };
            Assert.AreEqual(0, dep.HasErrors(items).Count);
            Assert.IsFalse(string.IsNullOrWhiteSpace(dep.HintText(items, a.ComponentType)));
            Assert.IsFalse(string.IsNullOrWhiteSpace(dep.HintText(items, b.ComponentType)));
            Assert.IsTrue(string.IsNullOrWhiteSpace(dep.HintText(items, ComponentType.Miscellaneous)));

            var key = b.Specs.Keys.FirstOrDefault();
            if (key != null)
            {
                b.Specs[key] = invalidSpec;
            }
            Assert.AreEqual(2, dep.HasErrors(items).Count);

            b.Specs.Clear();
            Assert.AreEqual(0, dep.HasErrors(items).Count);

            if (key != null)
            {
                b.Specs[key] = invalidSpec;
            }
            a.Specs.Clear();
            Assert.AreEqual(0, dep.HasErrors(items).Count);
        }

        private void TestSimpleQuantityPermutations(BuildComponentDependency dep, Item a, Item b)
        {
            //Order of permutations
            //
            //given params     -> No Error
            //a empty          -> No Error
            //b empty          -> No Error

            var items = new List<Item>() { a, b };
            Assert.AreEqual(0, dep.HasErrors(items).Count);
            Assert.IsFalse(string.IsNullOrWhiteSpace(dep.HintText(items, a.ComponentType)));
            Assert.IsFalse(string.IsNullOrWhiteSpace(dep.HintText(items, b.ComponentType)));
            Assert.IsTrue(string.IsNullOrWhiteSpace(dep.HintText(items, ComponentType.Miscellaneous)));

            Assert.AreEqual(0, dep.HasErrors(items).Count);
            Assert.AreEqual(0, dep.HasErrors(new List<Item>() { a }).Count);
            Assert.AreEqual(0, dep.HasErrors(new List<Item>() { b }).Count);
        }

        [TestMethod]
        [TestCategory("Dependency Checks")]
        public void CPUSocketTest()
        {
            var dep = Get("CPU Socket");

            TestPermutations(dep, 
                new Item()
                {
                    Name = "CPU",
                    ComponentType = ComponentType.CPU,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Socket Type", "AM4" }
                    }
                },
                new Item()
                {
                    Name = "Motherboard",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Socket Type", "AM4" }
                    }
                }
            );
        }

        [TestMethod]
        [TestCategory("Dependency Checks")]
        public void CPUChipsetTest()
        {
            var dep = Get("CPU Chipset");

            TestPermutations(dep,
                new Item()
                {
                    Name = "CPU",
                    ComponentType = ComponentType.CPU,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Compatibility", "AMD X470\nAMD B450\nAMD X570\nAMD B550" }
                    }
                },
                new Item()
                {
                    Name = "Motherboard",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        {"North Bridge", "AMD X570" }
                    }
                }
            );
        }

        [TestMethod]
        [TestCategory("Dependency Checks")]
        public void MemorySpeedTest()
        {
            var dep = Get("Motherboard Memory Speed");

            TestPermutations(dep,
                new Item()
                {
                    Name = "Motherboard",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Memory Speeds Supported", "4400(O.C)/3466(O.C.)/3400(O.C.)/3200(O.C.)/3000(O.C.)/2933(O.C.)/2800(O.C.)/2666/2400/2133" }
                    }
                },
                new Item()
                {
                    Name = "RAM",
                    ComponentType = ComponentType.RAM,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Memory Speed (MHz)", "DDR4-3200" }
                    }
                }
            );
        }

        [TestMethod]
        [TestCategory("Dependency Checks")]
        public void MotherboardFormFactorTest()
        {
            var dep = Get("Motherboard Form Factor");

            TestPermutations(dep,
                new Item()
                {
                    Name = "Motherboard",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Form Factor", "microATX" }
                    }
                },
                new Item()
                {
                    Name = "Case",
                    ComponentType = ComponentType.Case,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Motherboard Support", "ATX\nmicroATX\nMini-ITX" }
                    }
                }
            );
        }

        [TestMethod]
        [TestCategory("Dependency Checks")]
        public void M2FormFactor()
        {
            var dep = Get("SSD Form Factor");

            TestPermutations(dep,
                new Item()
                {
                    Name = "Motherboard",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        {"M.2 Port Type", "2242\n2260\n2280\n22110" }
                    }
                },
                new Item()
                {
                    Name = "SSD",
                    ComponentType = ComponentType.SSD,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Form Factor", "M.2 2280 M Key" }
                    }
                }
            );
        }

        [TestMethod]
        [TestCategory("Dependency Checks")]
        public void MotherboardMaxMemory()
        {
            var dep = Get("Motherboard Max Memory");

            TestPermutations(dep,
                new Item()
                {
                    Name = "Motherboard",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Max Memory Supported", "128GB" }
                    }
                },
                new Item()
                {
                    Name = "RAM",
                    ComponentType = ComponentType.RAM,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Memory Capacity", "16GB (2 x 8GB)" }
                    }
                }
            );
        }

        [TestMethod]
        [TestCategory("Dependency Checks")]
        public void MotherboardRamSlots()
        {
            var dep = Get("Motherboard RAM Slots");

            TestPermutations(dep,
                new Item()
                {
                    Name = "Motherboard",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Memory Slots", "4 x 288pin DIMM" }
                    }
                },
                new Item()
                {
                    Name = "RAM",
                    ComponentType = ComponentType.RAM,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Number of Modules", "2" }
                    }
                },
                "6"
            );
        }

        [TestMethod]
        [TestCategory("Dependency Checks")]
        public void HasOS()
        {
            var dep = Get("Has OS");

            TestSimpleQuantityPermutations(dep,
                new Item()
                {
                    Name = "OS",
                    ComponentType = ComponentType.OperatingSystem,
                    Specs = new Dictionary<string, string>()
                    {
                    }
                },
                new Item()
                {
                    Name = "Motherboard",
                    ComponentType = ComponentType.Motherboard,
                    Specs = new Dictionary<string, string>()
                    {
                    }
                }
            );
        }

        [TestMethod]
        [TestCategory("Dependency Checks")]
        public void GPULength()
        {
            var dep = Get("Case GPU Length");

            TestPermutations(dep,
                new Item()
                {
                    Name = "Case",
                    ComponentType = ComponentType.Case,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Max Video Card Length", "15.12 in. (384.00 mm)" }
                    }
                },
                new Item()
                {
                    Name = "GPU",
                    ComponentType = ComponentType.GPU,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Video Card Length", "12.72 in. (323.00 mm)" }
                    }
                },
                "17.00 in (whatever mm)"
            );
        }

        [TestMethod]
        [TestCategory("Dependency Checks")]
        public void HeatsinkHeight()
        {
            var dep = Get("Case CPU Heatsink Height");

            TestPermutations(dep,
                new Item()
                {
                    Name = "Case",
                    ComponentType = ComponentType.Case,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Max CPU Heatsink Height", "6.93 in. (176.00 mm)" }
                    }
                },
                new Item()
                {
                    Name = "Heatsink",
                    ComponentType = ComponentType.CPUCooler,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Heatsink Height", "6.26 in. (159.00 mm)" }
                    }
                },
                "17.00 in (whatever mm)"
            );
        }

        [TestMethod]
        [TestCategory("Dependency Checks")]
        public void PSUDepth()
        {
            var dep = Get("Case PSU Max Depth");

            TestPermutations(dep,
                new Item()
                {
                    Name = "Case",
                    ComponentType = ComponentType.Case,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Max Power Supply Depth", "7.28 in. (185.00 mm)" }
                    }
                },
                new Item()
                {
                    Name = "Power Supply",
                    ComponentType = ComponentType.PowerSupply,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Power Supply Depth", "5.91 in. (150.00 mm)" }
                    }
                },
                "17.00 in (whatever mm)"
            );
        }

        [TestMethod]
        [TestCategory("Dependency Checks")]
        public void GPUWattage()
        {
            var dep = Get("GPU Recommended PSU");

            TestPermutations(dep,
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
                    Name = "Power Supply",
                    ComponentType = ComponentType.PowerSupply,
                    Specs = new Dictionary<string, string>()
                    {
                        {"Wattage", "650 Watts" }
                    }
                },
                "450 Watts"
            );
        }

        [TestMethod]
        [TestCategory("Dependency Checks")]
        public void Case35DriveSlots()
        {
            var dep = Get("Case 3.5\" Drive Bay Quantity");
            var hdd = new Item()
            {
                Name = "HDD",
                ComponentType = ComponentType.HDD,
                Specs = new Dictionary<string, string>()
                    {
                        {"Recommended Power Supply", "500 Watts" }
                    }
            };
            var case_ = new Item()
            {
                Name = "Power Supply",
                ComponentType = ComponentType.Case,
                Specs = new Dictionary<string, string>()
                    {
                        {"Internal 3.5\" Drive Bays", "2" }
                    }
            };

            Assert.AreEqual(0, dep.HasErrors(new List<Item>() { case_ }).Count);
            Assert.AreEqual(0, dep.HasErrors(new List<Item>() { hdd, case_ }).Count);
            Assert.AreEqual(0, dep.HasErrors(new List<Item>() { hdd, hdd, case_ }).Count);
            Assert.AreEqual(4, dep.HasErrors(new List<Item>() { hdd, hdd, hdd, case_ }).Count);
        }
    }
}
