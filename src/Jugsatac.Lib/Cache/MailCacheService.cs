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
        internal MailCacheService(MailSyncIdentifier identifier)
        {
            this.identifier = identifier;
            cachedAccount = new CacheAccountItem()
            {
                MailAccount = identifier.Username,
                Mailbox = identifier.Mailbox,
                CachedMails = new List<CacheMailItem>()
            };
        }

        internal CacheMailItem GetCachedMail(uint mailid)
        {
            return cachedAccount.CachedMails.FirstOrDefault(m => m.MailId == mailid);
        }

        internal void UpdateCachedMailPart(uint mailid, string partName, string value)
        {
            var prop = typeof(CacheMailItem).GetProperties().FirstOrDefault(p => p.Name == partName);

            var cachedMail = GetCachedMail(mailid);
            if (cachedMail == null)
            {
                cachedMail = new CacheMailItem();
                cachedMail.MailId = mailid;
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
