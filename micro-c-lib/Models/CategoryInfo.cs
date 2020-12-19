using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace micro_c_lib.Models
{
    public class CategoryInfo
    {
        public string Name { get; set; }
        [JsonProperty(PropertyName = "item")]
        public string Url { get; set; }
        private string filter;
        public string Filter
        {
            get
            {
                if (string.IsNullOrWhiteSpace(filter))
                {
                    try
                    {
                        filter = Regex.Match(Url, FilterRegex).Groups[1].Value;
                    }
                    catch (Exception e)
                    {

                    }
                }

                return filter;
            }
        }
        public const string FilterRegex = "category\\/(.*?)\\/";
    }
}
