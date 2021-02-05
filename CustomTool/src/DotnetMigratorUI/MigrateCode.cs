using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DotnetMigratorUI
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

        /// <summary>
        /// This method will replace .NET Framework specific supported code snippet to .NET Core Supported.
        /// </summary>
        /// <param name="projectDirectory">.csproj file sirectory</param>
        public static void ReplaceWithDotnetCoreSupportedCodeSnipets(string projectDirectory)
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

        /// <summary>
        /// This method will create the additional class file required for .NET Core such as Program.cs, Startup.cs etc
        /// </summary>
        /// <param name="projectDirectory">.csproj file directory</param>
        public static void CreateAdditionalFiles(string projectDirectory)
        {
            File.Copy("Program_Template.cs", Path.Combine(projectDirectory, "Program.cs"), true);
            File.Copy("Startup_Template.cs", Path.Combine(projectDirectory, "Startup.cs"), true);
        }

        /// <summary>
        /// This method will remove .NET Core Obsolete files.
        /// </summary>
        /// <param name="projectDirectory">.csproj file directory</param>
        public static void RemoveObsoleteDotnetCoreFiles(string projectDirectory)
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

        /// <summary>
        /// This method will replace Style Tag in views (.cshtml) to .NET Core supported formats.
        /// </summary>
        /// <param name="projectDirectory">.csproj file directory</param>
        public static void ReplaceStyleTaginUI(string projectDirectory)
        {
            var scriptTag = "@Styles.Render(\"";
            var tagtype = "css";
            var scriptTagTemplate = "<link rel=\"stylesheet\" href=\"~/{0}\" />";
            ReplaceHtlTags(projectDirectory, scriptTag, tagtype, scriptTagTemplate);
        }

        /// <summary>
        /// This method will replace Script Tag in views (.cshtml) to .NET Core supported formats.
        /// </summary>
        /// <param name="projectDirectory">.csproj file directory</param>
        public static void ReplaceScriptTaginUI(string projectDirectory)
        {
            var scriptTag = "@Scripts.Render(\"";
            var tagtype = "js";
            var scriptTagTemplate = "<script src=\"~/{0}\"></script>";
            ReplaceHtlTags(projectDirectory, scriptTag, tagtype, scriptTagTemplate);
        }

        /// <summary>
        /// This method will migrate HttpContext session supported by .NET framework to .NET Core.
        /// </summary>
        /// <param name="projectDirectory">.csproj file directory</param>
        public static void ReplaceHttpContextSession(string projectDirectory)
        {
            var files = Directory.GetFiles(projectDirectory, "*.cshtml"
                                   , SearchOption.AllDirectories);
            var splitTag = "HttpContext.Current.Session[";
            var contextNamespace = "httpContextAccessor";
            var namespaceToAdd = $"@inject IHttpContextAccessor {contextNamespace}";
            var tagTobeReplaced = "HttpContext.Session.GetString({0})";

            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                var isModified = false;
                if (content.Contains(splitTag))
                {
                    var count = 0;
                    var scriptTagSplits = content.Split(new string[] { splitTag }, StringSplitOptions.RemoveEmptyEntries);

                    if (scriptTagSplits?.Length > 1)
                    {
                        content = $"{namespaceToAdd}{Environment.NewLine}{content}";
                        isModified = true;
                    }

                    foreach (var scriptTagSplit in scriptTagSplits)
                    {
                        if (count != 0)
                        {
                            var variableName = scriptTagSplit.Substring(0, scriptTagSplit.IndexOf(']'));
                            var tagToReplace = $"{splitTag}{variableName}]";
                            if(content.Contains(tagToReplace))
                            {
                                content = content.Replace(tagToReplace, $"@{contextNamespace}.{string.Format(tagTobeReplaced, variableName)}");
                                isModified = true;
                            }
                        }
                        count = count + 1;
                    }

                    if (isModified)
                    {
                        File.WriteAllText(file, content);
                    }
                }
            }
        }

        /// <summary>
        /// This method will convert Web.config file to ApplicationSettings.json file using dotnet-config2json tool
        /// </summary>
        /// <param name="projectDirectory">.csproj file directory</param>
        public static void MigrateWebconfigToAppsettings(string projectDirectory)
        {
            if (File.Exists(Path.Combine(projectDirectory, "Web.config")))
            {
                ExecuteCommand("dotnet tool install --global dotnet-config2json", 5000);
                ExecuteCommand($"dotnet config2json \"{Path.Combine(projectDirectory, "Web.config")}\"", 5000);

                if (File.Exists(Path.Combine(projectDirectory, "Web.json")))
                {
                    File.Move(Path.Combine(projectDirectory, "Web.json"), Path.Combine(projectDirectory, "appsettings.json"));
                    File.Delete(Path.Combine(projectDirectory, "Web.json"));
                    File.Delete(Path.Combine(projectDirectory, "Web.config"));
                }
            }
        }

        /// <summary>
        /// This method will move static images folder to wwwroot folder.
        /// </summary>
        /// <param name="projectDirectory">.csproj file directory</param>
        public static void MoveImagesFolderToWwwRootFolder(string projectDirectory)
        {
            if(Directory.Exists(Path.Combine(projectDirectory, "Images")))
            {
                if(!Directory.Exists(Path.Combine(projectDirectory, "wwwroot")))
                {
                    Directory.CreateDirectory(Path.Combine(projectDirectory, "wwwroot"));
                }
                Directory.Move(Path.Combine(projectDirectory, "Images"), Path.Combine(projectDirectory, "wwwroot", "Images"));
            }
        }

        private static void ReplaceHtlTags(string projectDirectory, string scriptTag, string tagtype, string scriptTagTemplate)
        {
            var files = Directory.GetFiles(projectDirectory, "*.cshtml"
                                    , SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                var isModified = false;
                if (content.Contains(scriptTag))
                {
                    var count = 0;
                    var scriptTagSplits = content.Split(new string[] { scriptTag },StringSplitOptions.RemoveEmptyEntries);

                    foreach (var scriptTagSplit in scriptTagSplits)
                    {
                        if (count != 0)
                        {
                            var scriptPath = scriptTagSplit.Substring(0, scriptTagSplit.IndexOf('"'));

                            var replaceText = $"{scriptTag}{scriptPath}\")";
                            //var replacedText = $"<script src=\"~/{tagtype}{scriptPath.Replace("~", string.Empty)}.{tagtype}\"></script>";
                            var replacedText = string.Format(scriptTagTemplate, $"{tagtype}{scriptPath.Replace("~", string.Empty)}.{tagtype}");
                            if (content.Contains(replaceText))
                            {
                                content = content.Replace(replaceText, replacedText);
                                isModified = true;
                            }
                        }

                        count = count + 1;
                    }
                }
                if (isModified)
                {
                    File.WriteAllText(file, content);
                }
            }
        }

        private static int ExecuteCommand(string commnd, int timeout)
        {
            var pp = new ProcessStartInfo("cmd.exe", "/C" + commnd)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = "C:\\",
            };
            using (var process = Process.Start(pp))
            {
                process.WaitForExit();
                process.Close();
            };
            return 0;
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
