using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace micro_c_lib.Models
{
    public class CategoryInfo
    {
        public string Name { get; set; }
        [JsonProperty(PropertyName = "item")]
        public string Url { get; set; }
    }
}
