using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static MicroCLib.Models.BuildComponent;
using System.Linq;

namespace micro_c_lib.Models.Build
{
    public class MemorySpeedDependency : FieldContainsDependency
    {
        public MemorySpeedDependency(string name, ComponentType first, string firstField, ComponentType second, string secondField)
            : base(name, first, firstField, second, secondField)
        {

        }

        protected override bool Compatible(string firstValue, string secondValue)
        {
            var firstSpeeds = ProcessSpeedString(firstValue);
            var secondSpeeds = ProcessSpeedString(secondValue);

            return firstSpeeds.Any(s1 => secondSpeeds.Any(s2 => s2 == s1));
        }

        private IEnumerable<string> ProcessSpeedString(string value)
        {
            var matches = Regex.Matches(value, "(\\d+)");
            return matches.OfType<Match>().Select(m => m.Groups[1].Value);
        }
    }
}