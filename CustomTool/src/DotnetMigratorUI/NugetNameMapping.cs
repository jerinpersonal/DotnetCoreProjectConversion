using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DotnetMigratorUI
{
    public static class NugetNameMapping
    {
        private static readonly List<NugetPackageName> _nugetPackageNames = null;
        static NugetNameMapping()
        {
            try
            {
                _nugetPackageNames = JsonConvert.DeserializeObject<List<NugetPackageName>>(File.ReadAllText("NugetNameMapping.json"));
            }
            catch
            {

            }
        }

        public static NugetPackageName GetCorePackage(string netframeWorkPackageName)
        {
            return _nugetPackageNames?.Find(x => x.netFrameWork == netframeWorkPackageName);
        }
    }

    public class NugetPackageName    {
        public string netFrameWork { get; set; } 
        public string dotnetCore { get; set; } 
        public string defaultCoreVersion { get; set; } 
    }
}
