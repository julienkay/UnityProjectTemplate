using System;

namespace Doji.PackageAuthoring.Editor {
    public partial class PackageCreationWizard {
        public string GetAssemblyInfo() {
            return $@"using System.Reflection;

[assembly: AssemblyTitle(""{_packageSettings.AssemblyName}"")]
[assembly: AssemblyDescription(""{_packageSettings.Description}"")]
[assembly: AssemblyCompany(""{_projectSettings.CompanyName}"")]
[assembly: AssemblyProduct(""{_packageSettings.AssemblyName}"")]
[assembly: AssemblyCopyright(""Copyright (c) {_packageSettings.Author} {DateTime.Now.Year}"")]

[assembly: AssemblyVersion(""{_projectSettings.Version}.0"")]
[assembly: AssemblyFileVersion(""{_projectSettings.Version}.0"")]
[assembly: AssemblyInformationalVersion(""{_projectSettings.Version}"")]
";
        }
    }
}
