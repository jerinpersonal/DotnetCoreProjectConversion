using System;
using System.Collections.Generic;
using System.Text;

namespace DotnetFrameworkToCoreProjectFileMigration
{
    public static class BundleConfigManager
    {
        private static List<BundleConfig> bundleConfigs = new List<BundleConfig>();
        public static List<BundleConfig> ProcessBundleConfig(string BundleConfigClassContent)
        {
            ProcessBundleConfigContent(BundleConfigClassContent, "ScriptBundle");
            ProcessBundleConfigContent(BundleConfigClassContent, "StyleBundle");

            return bundleConfigs;
        }

        private static List<BundleConfig> ProcessBundleConfigContent(string BundleConfigClassContent, string bundleName)
        {
            var bundleType = GetBundleType(bundleName);
            var scriptBundles = BundleConfigClassContent.Split(bundleName);
            int count = 0;
            foreach (var scriptBundle in scriptBundles)
            {
                if (count !=0 && scriptBundle?.Contains("Include") == true)
                {
                    var scriptBundleContent = scriptBundle.Split("Include");
                    if (scriptBundleContent.Length >= 2)
                    {
                        var outFilePath = scriptBundleContent[0]
                            .Replace("(", string.Empty)
                            .Replace("\"", string.Empty)
                            .Replace("~", string.Empty)
                            .Replace(")", string.Empty)
                            .Replace(".", string.Empty);

                        var bundleConfig = new BundleConfig();

                        if(bundleType == "js")
                        {
                            ScriptBundleConfig scriptBundleConfig = new ScriptBundleConfig();
                            scriptBundleConfig.outputFileName = $"wwwroot/{bundleType}{ outFilePath }.{bundleType}";
                            scriptBundleConfig.minify = new Minify();
                            bundleConfig = scriptBundleConfig;
                        }
                        else
                        {
                            bundleConfig.outputFileName = $"wwwroot/{bundleType}{ outFilePath }.{bundleType}";
                        }

                        if (scriptBundleContent[1].Contains(')'))
                            scriptBundleContent[1] = scriptBundleContent[1].Remove(scriptBundleContent[1].IndexOf(')'));

                        var inputFiles = new List<string>();
                        if (scriptBundleContent[1].Contains(","))
                        {
                            var splitInputFiles = scriptBundleContent[1].Split(",");
                            foreach (var splitInputFile in splitInputFiles)
                            {
                                inputFiles.Add(RemoveUnwantedCharacters(splitInputFile));
                            }
                        }
                        else
                        {
                            inputFiles.Add(RemoveUnwantedCharacters(scriptBundleContent[1]));
                        }
                        bundleConfig.inputFiles = inputFiles;
                        bundleConfigs.Add(bundleConfig);
                    }
                }

                count = count + 1;
            }
            return bundleConfigs;
        }

        
        private static string GetBundleType(string bundleName)
        {
            string bundleType = string.Empty;
            switch (bundleName)
            {
                case "ScriptBundle":
                    bundleType = "js";
                    break;
                case "StyleBundle":
                    bundleType = "css";
                    break;
            }
            return bundleType;
        }

        private static string RemoveUnwantedCharacters(string content)
        {
            if (content.Contains(')'))
                content = content.Remove(content.IndexOf(')'));
            content = content.Replace("(", string.Empty)
                        .Replace("\"", string.Empty)
                        .Replace("~", string.Empty)
                        .Replace(")", string.Empty)?.Trim();
            if(content.StartsWith('/'))
            {
                content = content.Remove(0, 1);
            }
            return content;
        }
    }

    public class Minify
    {
        public bool enabled { get; set; } = true;
        public bool renameLocals { get; set; } = true;
    }

    public class BundleConfig
    {
        public string outputFileName { get; set; }
        public List<string> inputFiles { get; set; }
        
    }

    public class ScriptBundleConfig : BundleConfig
    {
        public Minify minify { get; set; }
        public bool sourceMap { get; set; } = false;
    }
}
