using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jugsatac.Serialization
{
    class CacheAccountItemDto
    {
        [JsonProperty("mailAccount")]
        public string MailAccount { get; set; }
        [JsonProperty("mailbox")]
        public string Mailbox { get; set; }
        [JsonProperty("caches")]
        public IList<CacheMailItemDto> CachedMails { get; set; }
    }
}
