using Jugsatac.Lib;
using Jugsatac.Lib.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Jugsatac.Serialization;

namespace Jugsatac.Cache
{
    class JsonCachePersistence : ICachePersistence
    {
        private string filename;
        public JsonCachePersistence(string filename)
        {
            this.filename = filename;
        }

        public CacheAccountItem GetPersistentCache(MailSyncIdentifier identifier)
        {
            var jsonText = File.ReadAllText(filename);
            var cacheAccountDto = JsonConvert.DeserializeObject<CacheAccountItem>(jsonText);

            var cacheAccount = new CacheAccountItem()
            {
                Mailbox = cacheAccountDto.Mailbox,
                MailAccount = cacheAccountDto.MailAccount,
                CachedMails = (from m in cacheAccountDto.CachedMails select new CacheMailItem() { MailId = m.MailId, BodyText = m.BodyText }).ToList()
            };
            return cacheAccount;
        }

        public void SavePersistence(CacheAccountItem cache)
        {
            var cacheAccountDto = new CacheAccountItemDto()
            {
                Mailbox = cache.Mailbox,
                MailAccount = cache.MailAccount,
                CachedMails = (from m in cache.CachedMails select new CacheMailItemDto() { MailId = m.MailId, BodyText = m.BodyText }).ToList()
            };
            var jsonText = JsonConvert.SerializeObject(cacheAccountDto);
            File.WriteAllText(filename, jsonText);
        }
    }
}
