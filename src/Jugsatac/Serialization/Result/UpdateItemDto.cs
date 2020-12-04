using Jugsatac.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jugsatac.Serialization
{
    class UpdateItemDto
    {
        [JsonProperty("names")]
        public List<string> Names { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("updatedTime")]
        public DateTime UpdatedTime { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }

    }

    static class UpdateItemExtension
    {
        public static UpdateItem ConvertBack(this UpdateItemDto item)
        {
            return new UpdateItem()
            {
                Comment = item.Comment,
                Hash = item.Hash,
                Names = item.Names,
                UpdatedTime = item.UpdatedTime
            };
        }

        public static UpdateItemDto SerializeJson(this UpdateItem item)
        {
            return new UpdateItemDto()
            {
                Comment = item.Comment,
                Hash = item.Hash,
                Names = item.Names,
                UpdatedTime = item.UpdatedTime
            };
        }
    }
}
