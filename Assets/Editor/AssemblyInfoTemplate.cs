using System;

public partial class PackageScaffolder {
    public string GetAssemblyInfo() {
        return $@"using System.Reflection;

[assembly: AssemblyTitle(""{assemblyName}"")]
[assembly: AssemblyDescription(""{description}"")]
[assembly: AssemblyCompany(""{companyName}"")]
[assembly: AssemblyProduct(""{assemblyName}"")]
[assembly: AssemblyCopyright(""Copyright © {author} {DateTime.Now.Year}"")]

[assembly: AssemblyVersion(""{version}.0"")]
[assembly: AssemblyFileVersion(""{version}.0"")]
[assembly: AssemblyInformationalVersion(""{version}"")]
";
    }
}
