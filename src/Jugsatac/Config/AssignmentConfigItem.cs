using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Jugsatac.Config
{
    public class AssignmentConfigItem
    {
        [JsonProperty("name")]
        public string Name { get; internal set; }
        [JsonProperty("identifierPattern")]
        public string IdentifierPattern { get; internal set; }
        [JsonProperty("submitterPattern")]
        public string SubmitterPattern { get; internal set; }
        [JsonProperty("onlySubject")]
        public bool FetchSubjectOnly { get; internal set; }
        [JsonProperty("hideSubmitterName")]
        public bool HideSubmitterName { get; internal set; }

    }
}
