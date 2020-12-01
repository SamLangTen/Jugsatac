using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.IO;

namespace Jugsatac.Lib.Cache
{
    public class MailCacheService
    {

        private CacheAccountItem cachedAccount;

        public MailCacheService()
        {
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

        public void SaveCacheToFile(string filename)
        {
            var jsonContent = JsonConvert.SerializeObject(cachedAccount);
            File.WriteAllText(filename, jsonContent);
        }

        public void LoadCacheFromFile(string filename)
        {
            var jsonContent = File.ReadAllText(filename);
            cachedAccount = JsonConvert.DeserializeObject<CacheAccountItem>(jsonContent);
        }
    }
}
