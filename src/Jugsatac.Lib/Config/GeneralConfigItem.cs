using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jugsatac.Lib.Config
{
    public class GeneralConfigItem
    {
        [JsonProperty("host")]
        public string Host { get; set; }
        [JsonProperty("port")]
        public int Port { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("mailbox")]
        public string MailBox { get; set; }
        [JsonProperty("assignments")]
        public IList<AssignmentConfigItem> Assignments { get; set; }
    }
}
