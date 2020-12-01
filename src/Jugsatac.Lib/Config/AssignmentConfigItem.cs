using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Jugsatac.Lib.Config
{
    public class AssignmentConfigItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("identifierPattern")]
        public string IdentifierPattern { get; set; }
        [JsonProperty("submitterPattern")]
        public string SubmitterPattern { get; set; }
        [JsonProperty("onlySubject")]
        public bool FetchSubjectOnly { get; set; }
        [JsonProperty("hideSubmitterName")]
        public bool HideSubmitterName { get; set; }

    }
}
