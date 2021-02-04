using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            File.Copy("Program_Template.cs", Path.Combine(projectDirectory, "Program.cs"), true);
            File.Copy("Startup_Template.cs", Path.Combine(projectDirectory, "Startup.cs"), true);
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

        public static void ReplaceStyleTaginUI(string projectDirectory)
        {
            var scriptTag = "@Styles.Render(\"";
            var tagtype = "css";
            var scriptTagTemplate = "<link rel=\"stylesheet\" href=\"~/{0}\" />";
            ReplaceHtlTags(projectDirectory, scriptTag, tagtype, scriptTagTemplate);
        }

        public static void ReplaceScriptTaginUI(string projectDirectory)
        {
            var scriptTag = "@Scripts.Render(\"";
            var tagtype = "js";
            var scriptTagTemplate = "<script src=\"~/{0}\"></script>";
            ReplaceHtlTags(projectDirectory, scriptTag, tagtype, scriptTagTemplate);
        }

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
