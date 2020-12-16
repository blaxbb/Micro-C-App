using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace micro_c_lib.Models
{
    public class CategoryInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("item")]
        public string URL { get; set; }
    }
}
