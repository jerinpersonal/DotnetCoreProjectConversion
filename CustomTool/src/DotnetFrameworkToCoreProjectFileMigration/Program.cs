using NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace DotnetFrameworkToCoreProjectFileMigration
{
    class Program
    {
        private static string InputFilePath = @"Framework472.csproj";
        private static string coreTemplateFilePath = @"DotnetCore31Template.csproj";
        private static XNamespace msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";
        private static string packageConfigPath = @"packages.config";
        private static IEnumerable<PackageReference> packages = null;
        private static string executionDirectory = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Project file migration tool started.");
            InputFilePath = args?.Count() >= 1 ? args[0] : InputFilePath;
            //InputFilePath = @"C:\Workspace\XPO\mt-integrations-test\src\backend\Api\MT.Integrations.Backend.Api.Site.Tests\MT.Integrations.Backend.Api.Site.Tests.csproj";
            Console.WriteLine($"Input project file path is : { InputFilePath}");
            packageConfigPath = args?.Count() >= 2 ? args[1] : GetExcecutionPath(packageConfigPath);
            //packageConfigPath = @"C:\Workspace\XPO\mt-integrations\src\backend\Api\MT.Integrations.Backend.Api.Site\packages.config";
            Console.WriteLine($"Input package file path is : { packageConfigPath}");
            CreateCoreTemplateDocument();
            ProcessInputProject();
            Console.WriteLine("Project file migration tool completed.");
        }

        private static void ProcessInputProject()
        {
            var projDefinition = XDocument.Load(InputFilePath);

            var project = projDefinition?.Element(msbuild + "Project");
            var itemGroups = project?.Elements(msbuild + "ItemGroup");
            var document = CreateCoreTemplateDocument();
            var projectElement = document?.Element("Project");

            var references = itemGroups?.Elements(msbuild + "Reference");
            CreateReferences(references, projectElement);

            var projectReferences = itemGroups?.Elements(msbuild + "ProjectReference");
            CreateProjectReferences(projectReferences, projectElement);

            CreatePackageRefernces(projectElement);

            var renamedPath = Path.Combine(Path.GetDirectoryName(InputFilePath), $"Old_{Path.GetFileName(InputFilePath)}");
            Console.WriteLine($"Existing Project file renamed to : { renamedPath}");
            File.Move(InputFilePath, renamedPath);

            //var outputProjectFileMane = GetExcecutionPath($"New_{Path.GetFileName(InputFilePath)}");

            document.Save(InputFilePath);
        }

        private static void CreateReferences(IEnumerable<XElement> references, XElement project)
        {
            if (references != null)
            {
                var referenceItemGroup = new XElement("ItemGroup");
                foreach (var reference in references)
                {
                    var hintPath = reference.Element(msbuild + "HintPath");
                    if (hintPath == null)
                    {
                        if (string.IsNullOrEmpty(reference.Value))
                        {
                            var referenceElement = new XElement("Reference");
                            referenceElement.Add(reference.Attribute("Include"));
                            referenceItemGroup.Add(referenceElement);
                        }
                    }
                }
                project.Add(referenceItemGroup);
            }
        }

        private static void CreateProjectReferences(IEnumerable<XElement> references, XElement project)
        {
            if (references != null)
            {
                var element = new XElement("ItemGroup");
                foreach (var reference in references)
                {
                    var referenceElement = new XElement("ProjectReference");
                    referenceElement.Add(reference.Attribute("Include"));
                    element.Add(referenceElement);
                }
                project.Add(element);
            }
        }

        private static void CreatePackageRefernces(XElement project)
        {
            var packages = GetAllPackages();
            if (packages != null)
            {
                var packageReferenceItemGroup = new XElement("ItemGroup");
                foreach (var package in packages)
                {
                    var packageReferenceElement = new XElement("PackageReference");
                    packageReferenceElement.Add(new XAttribute("Include", package.Id));
                    var versionElement = new XElement("Version", package.Version.ToString());
                    packageReferenceElement.Add(versionElement);
                    packageReferenceItemGroup.Add(packageReferenceElement);
                }
                project.Add(packageReferenceItemGroup);
            }
        }

        private static IEnumerable<PackageReference> GetAllPackages()
        {
            if (packages == null)
            {
                var file = new PackageReferenceFile(packageConfigPath);
                packages = file.GetPackageReferences();
            }
            return packages;
        }

        private static string GetExcecutionPath(string fileName = null)
        {
            if (string.IsNullOrEmpty(executionDirectory))
            {
                executionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            return string.IsNullOrEmpty(fileName)? executionDirectory: Path.Join(executionDirectory, fileName);
        }

        private static XDocument CreateCoreTemplateDocument()
        {
            return XDocument.Load(coreTemplateFilePath);
        }
    }
}
