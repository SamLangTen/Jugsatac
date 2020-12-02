using System;
using System.Collections.Generic;
using System.Text;

namespace Jugsatac.Lib.Cache
{
    public interface ICachePersistence
    {
        CacheAccountItem GetPersistentCache(MailSyncIdentifier identifier);
        void SavePersistence(CacheAccountItem cache);

    }
}
