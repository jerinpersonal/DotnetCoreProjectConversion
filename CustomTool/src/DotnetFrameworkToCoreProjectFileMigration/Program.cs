using Newtonsoft.Json;
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
        private static string InputFilePath = @"D:\rajesh.pillai03\Data\RACKATHON_2020\Workspace\ToolTesting\legacy_master\migrators-legacy-01\src\eShopLegacyMVC\eShopLegacyMVC.csproj";

        static void Main(string[] args)
        {
            Console.WriteLine("Project file migration tool started.");
            InputFilePath = args?.Count() >= 1 ? args[0] : InputFilePath;
            //InputFilePath = @"C:\Workspace\XPO\mt-integrations-test\src\backend\Api\MT.Integrations.Backend.Api.Site.Tests\MT.Integrations.Backend.Api.Site.Tests.csproj";
            Console.WriteLine($"Input project file path is : { InputFilePath}");

            Migrator.Execute(InputFilePath, @"DotnetCore31Template.csproj");
            
            Console.WriteLine("Project file migration tool completed.");
        }
    }
}
