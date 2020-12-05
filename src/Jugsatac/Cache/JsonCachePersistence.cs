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
            //If cache does not exists, create it
            if (!File.Exists(filename))
            {
                return new CacheAccountItem()
                {
                    Mailbox = identifier.Mailbox,
                    MailAccount = identifier.Username,
                    CachedMails = new List<CacheMailItem>()
                };
            }

            //If exists, load it
            var jsonText = File.ReadAllText(filename);
            var cacheAccountDto = JsonConvert.DeserializeObject<CacheAccountItemDto>(jsonText);

            var cacheAccount = new CacheAccountItem()
            {
                Mailbox = cacheAccountDto.Mailbox,
                MailAccount = cacheAccountDto.MailAccount,
                LastUid = cacheAccountDto.LastUid,
                CachedMails = (from m in cacheAccountDto.CachedMails
                               select new CacheMailItem()
                               {
                                   MailId = m.MailId,
                                   BodyText = m.BodyText
                               }).ToList()
            };
            return cacheAccount;
        }

        public void SavePersistence(CacheAccountItem cache)
        {
            var cacheAccountDto = new CacheAccountItemDto()
            {
                Mailbox = cache.Mailbox,
                MailAccount = cache.MailAccount,
                LastUid = cache.LastUid,
                CachedMails = (from m in cache.CachedMails
                               select new CacheMailItemDto()
                               {
                                   MailId = m.MailId,
                                   BodyText = m.BodyText
                               }).ToList()
            };
            var jsonText = JsonConvert.SerializeObject(cacheAccountDto);
            File.WriteAllText(filename, jsonText);
        }
    }
}
