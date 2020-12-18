using System;
using System.Collections.Generic;
using System.Text;

namespace MicroCLib.Models
{
    public class ProgressInfo
    {
        public string Text { get; set; }
        public double Value { get; set; }

        public ProgressInfo(string text, double value)
        {
            Text = text;
            Value = value;
        }
    }
}
