using System;

namespace Doji.PackageAuthoring.Editor {
    public partial class PackageCreationWizard {
        public string GetAssemblyInfo() {
            return $@"using System.Reflection;

[assembly: AssemblyTitle(""{_assemblyName}"")]
[assembly: AssemblyDescription(""{_description}"")]
[assembly: AssemblyCompany(""{_companyName}"")]
[assembly: AssemblyProduct(""{_assemblyName}"")]
[assembly: AssemblyCopyright(""Copyright (c) {_author} {DateTime.Now.Year}"")]

[assembly: AssemblyVersion(""{_version}.0"")]
[assembly: AssemblyFileVersion(""{_version}.0"")]
[assembly: AssemblyInformationalVersion(""{_version}"")]
";
        }
    }
}
