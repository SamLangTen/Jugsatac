using System;
using Jugsatac.Lib.Config;
using Jugsatac.Lib.Cache;
using MimeKit;
using MailKit;

namespace Jugsatac.Lib
{
    public class MailSync
    {

        private GeneralConfigItem generalConfig;
        private MailCacheService cacheService;

        public MailSync(GeneralConfigItem config)
        {
            generalConfig = config;
            cacheService = new MailCacheService();
        }

        /// <summary>
        /// Get text body of mail from cache or server and update cache if available
        /// </summary>
        /// <param name="summary">Mail summary</param>
        /// <param name="inbox">Mailbox that contains mail</param>
        /// <returns>Body text</returns>
        private string GetBodyText(IMessageSummary summary, IMailFolder inbox)
        {
            //Get from cache
            var cachedText = cacheService?.GetCachedMail(summary.UniqueId.Id);
            if (cachedText != null)
                return cachedText.BodyText;

            //Download from server
            var bodyText = "";
            if (summary.TextBody != null)
            {
                var text = (TextPart)inbox.GetBodyPart(summary.UniqueId, summary.TextBody);
                bodyText = text.Text.Trim();
            }
            else if (summary.HtmlBody != null)
            {
                //TODO: Read html body
                bodyText = "";
            }

            //Update cache
            cacheService?.UpdateCachedMailPart(summary.UniqueId.Id, "BodyText", bodyText);

            return bodyText;
        }


    }
}
