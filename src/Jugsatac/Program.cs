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
                if (nextValue == "no" && i == args.Length - 1)
                {
                    DisplayUsage();
                    return;
                }
                else if (nextValue == "no")
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

        }
    }
}
