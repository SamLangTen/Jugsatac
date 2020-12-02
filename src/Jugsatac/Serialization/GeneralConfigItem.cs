using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Jugsatac.Config
{
    public class GeneralConfigItem
    {
        [JsonProperty("host")]
        public string Host { get; internal set; }
        [JsonProperty("port")]
        public int Port { get; internal set; }
        [JsonProperty("username")]
        public string Username { get; internal set; }
        [JsonProperty("password")]
        public string Password { get; internal set; }
        [JsonProperty("mailbox")]
        public string MailBox { get; internal set; }
        [JsonProperty("assignments")]
        public IList<AssignmentConfigItem> Assignments { get; set; }


        public static GeneralConfigItem LoadFromFile(string filename)
        {
            var jsonText = File.ReadAllText(filename);
            var config = JsonConvert.DeserializeObject<GeneralConfigItem>(jsonText);
            return config;
        }

    }
}
