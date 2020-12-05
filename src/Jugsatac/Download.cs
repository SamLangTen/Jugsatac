using Jugsatac.Config;
using Jugsatac.Lib;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Jugsatac
{
    public static class Download
    {
        public static void GenerateDownload(GeneralConfigItem config, string outputDirectory)
        {
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            var mailsync = new MailSync(config.Host, config.Port, config.Username, config.Password, config.MailBox, config.Assignments.Select(t => t.ConvertBack()).ToList());

            config.Assignments.ToList().ForEach(a =>
            {
                Directory.CreateDirectory(Path.Combine(outputDirectory, a.Name));
                mailsync.Download(a.ConvertBack(), Path.Combine(outputDirectory, a.Name));
            });

        }
    }
}
