using Jugsatac.Lib;
using Jugsatac.Config;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Jugsatac.Serialization;
using Newtonsoft.Json;
using Jugsatac.Cache;
using System.IO;
using Jugsatac.Lib.Cache;

namespace Jugsatac
{
    class Classification
    {
        private string GenerateClassificationJson(GeneralConfigItem config, string cacheFilename)
        {
            var mailsync = new MailSync(config.Host, config.Port, config.Username, config.Password, config.MailBox, config.Assignments.Select(t => t.ConvertBack()).ToList());

            ICachePersistence cache = null;
            if (cacheFilename != null)
            {
                cache = new JsonCachePersistence(cacheFilename);
                mailsync.CacheService.LoadCache(cache);
            }

            var assignments = (from ass in mailsync.Assignments
                               select new AssignmentUpdateItemDto()
                               {
                                   Name = ass.Name,
                                   Items = (from u in mailsync.Classify(ass) select u.SerializeJson()).ToList()
                               }).ToList();

            if (cacheFilename != null)
            {
                mailsync.CacheService.SaveCache(cache);
            }


            return JsonConvert.SerializeObject(assignments);
        }

        public void GenerateClassification(GeneralConfigItem config, string cacheFilename, string outputFilename, GeneratedResultFileType type)
        {
            var jsonText = "";
            if (type == GeneratedResultFileType.Json)
                jsonText = GenerateClassificationJson(config, cacheFilename);
            File.WriteAllText(outputFilename, jsonText);
        }
        public void GenerateClassification(GeneralConfigItem config, string cacheFilename, GeneratedResultFileType type)
        {
            var jsonText = "";
            if (type == GeneratedResultFileType.Json)
                jsonText = GenerateClassificationJson(config, cacheFilename);
            Console.WriteLine(jsonText);
        }
    }

    public enum GeneratedResultFileType
    {
        Json
    }
}
