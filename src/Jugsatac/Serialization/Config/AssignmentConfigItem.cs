using System;
using System.Collections.Generic;
using System.Text;
using Jugsatac.Lib;
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

    public static class AssignmentExtension
    {
        public static Assignment ConvertBack(this AssignmentConfigItem config)
        {
            return new Assignment()
            {
                HideSubmitterName = config.HideSubmitterName,
                IdentifierPattern = config.IdentifierPattern,
                Name = config.Name,
                SubjectOnly = config.FetchSubjectOnly,
                SubmitterPattern = config.SubmitterPattern
            };
        }

        public static AssignmentConfigItem SerializeJson(this Assignment config)
        {
            return new AssignmentConfigItem()
            {
                FetchSubjectOnly = config.SubjectOnly,
                HideSubmitterName = config.HideSubmitterName,
                IdentifierPattern = config.IdentifierPattern,
                SubmitterPattern = config.SubmitterPattern,
                Name = config.Name
            };
        }
    }
}
