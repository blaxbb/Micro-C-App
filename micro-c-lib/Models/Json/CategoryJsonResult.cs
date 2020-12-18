using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace micro_c_lib.Models.Json
{
    internal class CategoryJsonResult
    {
        [JsonProperty(PropertyName = "itemListElement")]
        public List<CategoryInfo> Categories { get; set; }
    }
}
