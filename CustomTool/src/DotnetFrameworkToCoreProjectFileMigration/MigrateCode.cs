using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DotnetFrameworkToCoreProjectFileMigration
{
    public static class MigrateCode
    {
        private static readonly List<ReplaceCode> _replaceCodes = null;
        static MigrateCode()
        {
            try
            {
                _replaceCodes = JsonConvert.DeserializeObject<List<ReplaceCode>>(File.ReadAllText("ReplaceCode.json"));
            }
            catch
            {

            }
        }
        public static void ReplaceCode(string projectDirectory)
        {
            if (_replaceCodes != null)
            {
                foreach (var replaceCode in _replaceCodes)
                {
                    var files = Directory.GetFiles(projectDirectory, replaceCode.FileExtensionsToReplace
                        , SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        var content = File.ReadAllText(file);
                        bool isModified = false;
                        foreach(var teplaceText in replaceCode.replaceTexts)
                        {
                            if (content.Contains(teplaceText.netFrameWork))
                            {
                                switch(teplaceText.action)
                                {
                                    case "AddNameSpace":
                                        content = $"{teplaceText.dotnetCore}{Environment.NewLine}{content}";
                                        isModified = true;
                                        break;
                                    default:
                                        content = content.Replace(teplaceText.netFrameWork, teplaceText.dotnetCore);
                                        isModified = true;
                                        break;
                                }
                            }
                        }

                        if (isModified)
                        {
                            File.WriteAllText(file, content);
                        }
                    }
                }
            }
        }

        public static void CreateAdditionalFiles(string projectDirectory)
        {
            File.Copy("Program_Template.cs", Path.Combine(projectDirectory, "Program.cs"));
            File.Copy("Startup_Template.cs", Path.Combine(projectDirectory, "Startup.cs"));
        }

        public static void RemoveUnwantedFiles(string projectDirectory)
        {
            if (Directory.Exists(Path.Combine(projectDirectory, "App_Start")))
            {
                Directory.Delete(Path.Combine(projectDirectory, "App_Start"), true);
            }
            if (Directory.Exists(Path.Combine(projectDirectory, "App_Data")))
            {
                Directory.Delete(Path.Combine(projectDirectory, "App_Data"), true);
            }
            if (Directory.Exists(Path.Combine(projectDirectory, "Properties")))
            {
                Directory.Delete(Path.Combine(projectDirectory, "Properties"), true);
            }

            if (File.Exists(Path.Combine(projectDirectory, "Global.asax")))
            {
                File.Delete(Path.Combine(projectDirectory, "Global.asax"));
            }
            if (File.Exists(Path.Combine(projectDirectory, "Global.asax.cs")))
            {
                File.Delete(Path.Combine(projectDirectory, "Global.asax.cs"));
            }
        }
    }

    public class ReplaceText
    {
        public string netFrameWork { get; set; }
        public string dotnetCore { get; set; }
        public string action { get; set; }
    }

    public class ReplaceCode
    {
        public string FileExtensionsToReplace { get; set; }
        public List<ReplaceText> replaceTexts { get; set; }
    }
}
