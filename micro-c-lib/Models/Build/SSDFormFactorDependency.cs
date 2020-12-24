using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static MicroCLib.Models.BuildComponent;

namespace micro_c_lib.Models.Build
{
    public class SSDFormFactorDependency : FieldContainsDependency
    {
        /*
         * 
         * We don't care about the form factor for SATA ssd's because errors for those drives
         * will be picked up by other dependencies, like sata ports on mobo and sata bays in case.
         * 
         */

        public SSDFormFactorDependency(string name, ComponentType first, string firstField, ComponentType second, string secondField)
            : base(name, first, firstField, second, secondField)
        {

        }

        protected override bool Compatible(string firstValue, string secondValue)
        {
            if(firstValue == "2.5\"" || secondValue == "2.5\"")
            {
                return true;
            }
            return base.Compatible(firstValue, secondValue);
        }
    }
}
