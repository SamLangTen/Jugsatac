using Jugsatac.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jugsatac
{
    class Program
    {
        static void DisplayUsage()
        {
            Console.WriteLine("Usage: Jugsatac [OPTION]... -s CONFIG_FILE");
            Console.WriteLine("   or: Jugsatac [OPTION]... -s CONFIG_FILE -o OUTPUT_FILE");
            Console.WriteLine("   or: Jugsatac [OPTION]... -s CONFIG_FILE -o OUTPUT_DIRECTORY -d");
            Console.WriteLine("In the 1st form, fetch and classify mails according to CONFIG_FILE and output to console.");
            Console.WriteLine("In the 2nd form, fetch and classify mails according to CONFIG_FILE and output to OUTPUT_FILE.");
            Console.WriteLine("In the 3rd form, download attachments and save to OUTPUT_DIRECTORY");
            Console.WriteLine("");
            Console.WriteLine("Options:");
            Console.WriteLine("  -c CACHE_FILE\t\t\tload persistence saved cache file.");
        }

        static void Main(string[] args)
        {
            //Parse args
            string cacheFilename = null;
            string outputFilename = null;
            string configFilename = null;
            bool isDownload = false;
            string nextValue = "no";
            for (int i = 0; i < args.Length; i++)
            {
                if (nextValue == "no")
                {
                    if (args[i] == "-c")
                        nextValue = "cache";
                    else if (args[i] == "-o")
                        nextValue = "output";
                    else if (args[i] == "-s")
                        nextValue = "config";
                    else if (args[i] == "-d")
                        isDownload = true;
                }
                else
                {
                    if (nextValue == "cache")
                    {
                        cacheFilename = args[i];
                        nextValue = "no";
                    }

                    else if (nextValue == "output")
                    {
                        outputFilename = args[i];
                        nextValue = "no";
                    }

                    else if (nextValue == "config")
                    {
                        configFilename = args[i];
                        nextValue = "no";
                    }

                }

            }

            //Check args
            if (configFilename == null)
            {
                DisplayUsage();
                return;
            }


            //Run
            if (!isDownload)
            {
                if (outputFilename == null)
                    Classification.GenerateClassification(GeneralConfigItem.LoadFromFile(configFilename), cacheFilename, GeneratedResultFileType.Json);
                else
                    Classification.GenerateClassification(GeneralConfigItem.LoadFromFile(configFilename), cacheFilename, outputFilename, GeneratedResultFileType.Json);
            }
            else
            {
                if (outputFilename == null)
                {
                    DisplayUsage();
                    return;
                }

                Download.GenerateDownload(GeneralConfigItem.LoadFromFile(configFilename), outputFilename);

            }

        }
    }
}
