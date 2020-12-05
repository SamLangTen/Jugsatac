using System;
using System.Collections.Generic;

namespace Jugsatac.Lib.Cache
{
    public class CacheAccountItem
    {
        public string MailAccount { get; set; }

        public string Mailbox { get; set; }
 
        public uint LastUid { get; set; }
        public IList<CacheMailItem> CachedMails { get; set; }
    }
}
