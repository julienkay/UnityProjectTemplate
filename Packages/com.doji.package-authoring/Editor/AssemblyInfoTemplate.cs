using System;

namespace Doji.PackageAuthoring.Editor {

public partial class PackageCreationWizard {
    public string GetAssemblyInfo() {
        return $@"using System.Reflection;

[assembly: AssemblyTitle(""{assemblyName}"")]
[assembly: AssemblyDescription(""{description}"")]
[assembly: AssemblyCompany(""{companyName}"")]
[assembly: AssemblyProduct(""{assemblyName}"")]
[assembly: AssemblyCopyright(""Copyright (c) {author} {DateTime.Now.Year}"")]

[assembly: AssemblyVersion(""{version}.0"")]
[assembly: AssemblyFileVersion(""{version}.0"")]
[assembly: AssemblyInformationalVersion(""{version}"")]
";
    }
}

}
