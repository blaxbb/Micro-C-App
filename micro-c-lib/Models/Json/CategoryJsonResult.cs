using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace micro_c_lib.Models.Json
{
    internal class CategoryJsonResult
    {
        [JsonPropertyName("itemListElement")]
        public List<CategoryInfo> Categories { get; set; }
    }
}
