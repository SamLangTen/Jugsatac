using System;
using System.Linq;
using System.Collections.Generic;
using Jugsatac.Lib.Cache;
using MimeKit;
using MailKit;
using MailKit.Net.Imap;
using Jugsatac.Lib.Exception;
using MailKit.Search;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Jugsatac.Lib
{
    public class MailSync
    {

        private List<Assignment> assignments;
        private MailSyncIdentifier identifier = new MailSyncIdentifier();

        public MailCacheService CacheService { get; private set; }

        public MailSync(string host, int port, string username, string password, string mailbox, List<Assignment> assignments)
        {
            identifier.Host = host;
            identifier.Port = port;
            identifier.Username = username;
            identifier.Password = password;
            identifier.Mailbox = mailbox;
            this.assignments = assignments.ToList();
            CacheService = new MailCacheService(identifier);
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
            var cachedText = CacheService?.GetCachedMail(summary.UniqueId.Id);
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
            CacheService?.UpdateCachedMailPart(summary.UniqueId.Id, "BodyText", bodyText);

            return bodyText;
        }

        /// <summary>
        /// Get all folders in client recursively
        /// </summary>
        private List<IMailFolder> RecursiveGetFolder(IMailFolder folder)
        {
            var folders = new List<IMailFolder>();
            foreach (var f in folder.GetSubfolders())
            {
                folders.Add(f);
                folders.AddRange(RecursiveGetFolder(f));
            }
            return folders;
        }

        /// <summary>
        /// Calculate hash of a string by SHA256
        /// </summary>
        /// <param name="original">string</param>
        private string CalcHash(string original)
        {
            using var hasher = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(original);
            var hash = hasher.ComputeHash(bytes);
            var sb = new StringBuilder();
            (from b in hash select b.ToString("X2")).ToList().ForEach(a => sb.Append(a));
            return sb.ToString();

        }

        /// <summary>
        /// Get identifier of a series of submitters. This identifier varies in different runs.
        /// </summary>
        /// <param name="submitters">A series of submitters</param>
        private int GetSubmittersIdentifier(List<string> submitters)
        {
            int hash = 0;
            submitters.ForEach(s =>
            {
                hash = HashCode.Combine(hash, s.GetHashCode());
            });
            return hash;
        }

        /// <summary>
        /// Get all available assignments
        /// </summary>
        public Assignment[] Assignments { get => assignments.ToArray(); }

        /// <summary>
        /// Classify specific assignment submission
        /// </summary>
        /// <param name="assignment">Assignment information</param>
        /// <returns>A list of UpdateItem</returns>
        public List<UpdateItem> Classify(Assignment assignment)
        {
            //Initialize IMAP Client
            using var client = new ImapClient();

            client.Connect(identifier.Host, identifier.Port, true);
            client.Authenticate(identifier.Username, identifier.Password);

            //Expand all mail folders
            var allFolders = from n in client.PersonalNamespaces from f in RecursiveGetFolder(client.GetFolder(n)) select f;
            var specificInBox = allFolders.FirstOrDefault(f => f.FullName == identifier.Mailbox);
            if (specificInBox == null)
                throw new MailboxNotFoundException();

            var inbox = specificInBox;
            inbox.Open(FolderAccess.ReadOnly);

            //Fetch summaries of all mail in specific mailbox
            var query = SearchQuery.All;
            var uids = inbox.Search(query);
            var summaries = inbox.Fetch(uids.ToList(), MessageSummaryItems.Envelope | MessageSummaryItems.BodyStructure);

            //Filter specific assignment
            var filteredCodeMessages = from s in summaries
                                       where Regex.IsMatch(s.Envelope.Subject, assignment.IdentifierPattern)
                                       select s;

            //Fetch body content
            //If a cache is available, it will try to get body text from cache
            var dict = filteredCodeMessages.ToDictionary(m => new UpdateItem()
            {
                Names = Regex.Matches(Regex.Replace(m.Envelope.Subject, assignment.IdentifierPattern, ""), assignment.SubmitterPattern).Select(t => t.Value).ToList(),
                UpdatedTime = m.Date.LocalDateTime,
                Comment = "",
                Hash = CalcHash(m.Envelope.From.FirstOrDefault().Name)
            }, m => m);
            var codeItems = dict.Keys;

            //For each submitter, only the sender mail address in the first mail
            //mentioned this submitter can update submission.
            //The following statements select the latest mail from the first sender for every submitter
            var codeGroupByNames = from c in codeItems orderby c.UpdatedTime ascending let tile = GetSubmittersIdentifier(c.Names) group c by tile;
            var availableCodeHashes = from g in codeGroupByNames select new { g.FirstOrDefault().Names, g.FirstOrDefault().Hash };
            var availableCodeItems = from c in codeItems from h in availableCodeHashes where GetSubmittersIdentifier(c.Names) == GetSubmittersIdentifier(h.Names) && c.Hash == h.Hash orderby c.UpdatedTime descending select c;
            var latestCodeItems = (from c in availableCodeItems let tile = GetSubmittersIdentifier(c.Names) group c by tile into groupByNames select groupByNames.FirstOrDefault()).ToList();
            latestCodeItems = (from c in latestCodeItems
                               select new UpdateItem()
                               {
                                   Names = c.Names,
                                   UpdatedTime = c.UpdatedTime,
                                   Hash = c.Hash,
                                   Comment = assignment.SubjectOnly ? "" : GetBodyText(dict[c], inbox)
                               }).ToList();


            //Hide submitters' name
            if (assignment.HideSubmitterName)
                latestCodeItems = (from c in latestCodeItems
                                   select new UpdateItem()
                                   {
                                       Comment = c.Comment,
                                       Hash = c.Hash,
                                       UpdatedTime = c.UpdatedTime,
                                       Names = (from n in c.Names select string.Join(null, Enumerable.Range(1, n.Length - 1).Select(c => "*")) + n.Last()).ToList()
                                   }).ToList();

            //Disconnect from IMAP server
            client.Disconnect(true);

            return latestCodeItems;
        }

        /// <summary>
        /// Download attachments after classifying submissions
        /// </summary>
        /// <param name="assignment">Assignments</param>
        /// <param name="targetDirectory">Directory contained downloaded attachments</param>
        public void Download(Assignment assignment, string targetDirectory)
        {
            //Initialize IMAP Client
            using var client = new ImapClient();

            client.Connect(identifier.Host, identifier.Port, true);
            client.Authenticate(identifier.Username, identifier.Password);

            //Expand all mail folders
            var allFolders = from n in client.PersonalNamespaces from f in RecursiveGetFolder(client.GetFolder(n)) select f;
            var specificInBox = allFolders.FirstOrDefault(f => f.FullName == identifier.Mailbox);
            if (specificInBox == null)
                throw new MailboxNotFoundException();

            var inbox = specificInBox;
            inbox.Open(FolderAccess.ReadOnly);

            //Fetch summaries of all mail in specific mailbox
            var query = SearchQuery.All;
            var uids = inbox.Search(query);
            var summaries = inbox.Fetch(uids.ToList(), MessageSummaryItems.Envelope | MessageSummaryItems.BodyStructure);

            //Filter specific assignment
            var filteredCodeMessages = from s in summaries
                                       where Regex.IsMatch(s.Envelope.Subject, assignment.IdentifierPattern)
                                       select s;

            //Fetch body content
            //If a cache is available, it will try to get body text from cache
            var codeItems = from m in filteredCodeMessages
                            select new
                            {
                                Update = new UpdateItem()
                                {
                                    Names = Regex.Matches(Regex.Replace(m.Envelope.Subject, assignment.IdentifierPattern, ""), assignment.SubmitterPattern).Select(t => t.Value).ToList(),
                                    UpdatedTime = m.Date.LocalDateTime,
                                    Comment = "",
                                    Hash = CalcHash(m.Envelope.From.FirstOrDefault().Name)
                                },
                                Uid = m.UniqueId
                            };


            //For each submitter, only the sender mail address in the first mail
            //mentioned this submitter can update submission.
            //The following statements select the latest mail from the first sender for every submitter
            var codeGroupByNames = from c in codeItems orderby c.Update.UpdatedTime ascending let tile = GetSubmittersIdentifier(c.Update.Names) group c by tile;
            var availableCodeHashes = from g in codeGroupByNames select new { g.FirstOrDefault().Update.Names, g.FirstOrDefault().Update.Hash };
            var availableCodeItems = from c in codeItems from h in availableCodeHashes where GetSubmittersIdentifier(c.Update.Names) == GetSubmittersIdentifier(h.Names) && c.Update.Hash == h.Hash orderby c.Update.UpdatedTime descending select c;
            var latestCodeItems = (from c in availableCodeItems let tile = GetSubmittersIdentifier(c.Update.Names) group c by tile into groupByNames select groupByNames.FirstOrDefault()).ToList();

            //Get filtered mail and download attachments.
            filteredCodeMessages = from m in filteredCodeMessages from c in latestCodeItems where m.UniqueId == c.Uid select m;
            filteredCodeMessages.ToList().ForEach(a =>
            {

                MimeMessage message = client.Inbox.GetMessage(a.UniqueId);
                foreach (MimeEntity attachment in message.Attachments)
                {
                    var fileName = Path.Combine(targetDirectory, attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name);
                    using (FileStream stream = File.Create(fileName))
                    {
                        if (attachment is MessagePart rfc822)
                        {
                            rfc822.Message.WriteTo(stream);
                        }
                        else if (attachment is MimePart part)
                        {
                            part.Content.DecodeTo(stream);
                        }
                    }
                    //Set write time as the time that receives mail.
                    File.SetLastWriteTime(fileName, a.Date.LocalDateTime);
                }

            });

        }
    }
}
