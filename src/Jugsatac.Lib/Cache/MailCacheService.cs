using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Jugsatac.Lib.Cache
{
    public class MailCacheService
    {

        private CacheAccountItem cachedAccount;
        private MailSyncIdentifier identifier;
        public MailCacheService(MailSyncIdentifier identifier)
        {
            this.identifier = identifier;
            cachedAccount = new CacheAccountItem();
            cachedAccount.CachedMails = new List<CacheMailItem>();
        }

        public CacheMailItem GetCachedMail(uint mailid)
        {
            return cachedAccount.CachedMails.FirstOrDefault(m => m.MailId == mailid);
        }

        public void UpdateCachedMailPart(uint mailid, string partName, string value)
        {
            var prop = typeof(CacheMailItem).GetProperties().FirstOrDefault(p => p.Name == partName);

            var cachedMail = GetCachedMail(mailid);
            if (cachedMail == null)
            {
                cachedMail = new CacheMailItem();
                cachedAccount.CachedMails.Add(cachedMail);
            }

            prop.SetValue(cachedMail, value);
        }

        public void SaveCache(ICachePersistence persistence)
        {
            persistence.SavePersistence(cachedAccount);
        }

        public void LoadCache(ICachePersistence persistence)
        {
            cachedAccount.CachedMails = cachedAccount.CachedMails.Union(persistence.GetPersistentCache(identifier)?.CachedMails).ToList();
        }

    }
}
