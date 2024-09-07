using System;

public static class AssemblyInfoTemplate {
    public static string GetContent(string assemblyName, string companyName, string author, string description, string version) {
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
