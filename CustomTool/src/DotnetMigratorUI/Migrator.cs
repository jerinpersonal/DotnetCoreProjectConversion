using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DotnetMigratorUI
{
    public static class Migrator
    {
        private static XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";
        public static void Execute(string projectFilePath, string templatePath)
        {
            ProcessInputProject(projectFilePath, templatePath);
        }

        private static void ProcessInputProject(string projectFilePath, string templatePath)
        {
            var executionDirectory = Path.GetDirectoryName(projectFilePath);
            var projDefinition = XDocument.Load(projectFilePath);

            var project = projDefinition?.Element(msbuild + "Project");
            var itemGroups = project?.Elements(msbuild + "ItemGroup");

            var document = CreateCoreTemplateDocument(templatePath);
            var projectElement = document?.Element("Project");

            var references = itemGroups?.Elements(msbuild + "Reference");
            CreateReferences(references, projectElement, executionDirectory);


            MigrateBundleConfig(executionDirectory);

            MigrateCode.ReplaceCode(executionDirectory);

            MigrateCode.CreateAdditionalFiles(executionDirectory);

            MigrateCode.RemoveUnwantedFiles(executionDirectory);


            MigrateCode.ReplaceScriptTaginUI(executionDirectory);

            MigrateCode.ReplaceStyleTaginUI(executionDirectory);


            var renamedPath = Path.Combine(executionDirectory, $"Old_{Path.GetFileName(projectFilePath)}");
            Console.WriteLine($"Existing Project file renamed to : { renamedPath}");
            File.Move(projectFilePath, renamedPath);


            document.Save(projectFilePath);
        }

        private static void MigrateBundleConfig(string projectDirectory)
        {
            //var projectDirectory = Path.GetDirectoryName(InputFilePath);
            var bundleConfigFilePath = Path.Combine(projectDirectory, "App_Start", "BundleConfig.cs");
            if (File.Exists(bundleConfigFilePath))
            {
                var bundleConfigText = File.ReadAllText(bundleConfigFilePath);
                var bundleConfig = BundleConfigManager.ProcessBundleConfig(bundleConfigText);
                if (bundleConfig?.Count > 0)
                {
                    File.WriteAllText(Path.Combine(projectDirectory, "bundleconfig.json")
                        , JsonConvert.SerializeObject(bundleConfig));
                }
            }
        }

        private static void CreateReferences(IEnumerable<XElement> references, XElement project, string projectDirectory)
        {
            var referenceItemGroup = new XElement("ItemGroup");
            if (references != null)
            {
                foreach (var reference in references)
                {
                    var packageName = GetPackageNameFromIncludeAttribute(
                        reference.Attributes("Include").FirstOrDefault()?.Value);
                    if (!string.IsNullOrEmpty(packageName))
                    {
                        var package = NugetNameMapping.GetCorePackage(packageName);
                        if (package != null)
                        {
                            var referenceElement = new XElement("PackageReference");
                            referenceElement.Add(new XAttribute("Include", package.dotnetCore),
                                new XAttribute("Version", package.defaultCoreVersion));
                            referenceItemGroup.Add(referenceElement);
                        }
                    }

                    if(packageName == "EntityFramework")
                    {
                        var referenceElement = new XElement("PackageReference");
                        referenceElement.Add(new XAttribute("Include", "Microsoft.EntityFrameworkCore.Relational"),
                            new XAttribute("Version", "5.0.2"));
                        referenceItemGroup.Add(referenceElement);

                        var referenceElementTools = new XElement("PackageReference");
                        referenceElementTools.Add(new XAttribute("Include", "Microsoft.EntityFrameworkCore.tools"),
                            new XAttribute("Version", "5.0.2"));
                        referenceItemGroup.Add(referenceElementTools);
                    }
                }

            }

            var bundleConfigFilePath = Path.Combine(projectDirectory, "App_Start", "BundleConfig.cs");
            if (File.Exists(bundleConfigFilePath))
            {
                var referenceElement = new XElement("PackageReference");
                referenceElement.Add(new XAttribute("Include", "BuildBundlerMinifier"),
                    new XAttribute("Version", "3.2.449"));
                referenceItemGroup.Add(referenceElement);
            }

            if (Directory.GetFiles(projectDirectory, "*.cshtml", SearchOption.AllDirectories)?.Count() > 0)
            {
                var referenceElement = new XElement("PackageReference");
                referenceElement.Add(new XAttribute("Include", "Microsoft.AspNetCore.Html.Abstractions"),
                    new XAttribute("Version", "2.2.0"));
                referenceItemGroup.Add(referenceElement);
            }

            project.Add(referenceItemGroup);
        }

        private static string GetPackageNameFromIncludeAttribute(string includeAttributeValue)
        {
            string packageName = null;
            var splits = includeAttributeValue.Split(',');
            if (splits?.Length > 1)
            {
                packageName = splits[0];
            }
            return packageName;
        }

        private static XDocument CreateCoreTemplateDocument(string templatePath)
        {
            return XDocument.Load(templatePath);
        }
    }
}
