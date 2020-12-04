using Newtonsoft.Json;
using Jugsatac.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jugsatac.Serialization
{
    class AssignmentUpdateItemDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("updates")]
        public List<UpdateItemDto> Items { get; set; }
    }
}
