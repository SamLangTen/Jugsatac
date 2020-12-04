using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jugsatac.Serialization
{
    class CacheMailItemDto
    {
        [JsonProperty("id")]
        public uint MailId { get; set; }
        [JsonProperty("bodyText")]
        public string BodyText { get; set; }

    }
}
