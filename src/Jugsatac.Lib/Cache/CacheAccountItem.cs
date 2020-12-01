using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Jugsatac.Lib.Cache
{
    public class CacheAccountItem
    {
        [JsonProperty("mailAccount")]
        public string MailAccount { get; set; }

        [JsonProperty("cachedMails")]
        public IList<CacheMailItem> CachedMails { get; set; }
    }
}
