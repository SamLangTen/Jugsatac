using System;
using Newtonsoft.Json;

namespace Jugsatac.Lib.Cache
{
    public class CacheMailItem
    {
        [JsonProperty("id")]
        public uint MailId { get; set; }

        [JsonProperty("body")]
        public string BodyText { get; set; }

        public CacheMailItem()
        {
        }


    }
}
