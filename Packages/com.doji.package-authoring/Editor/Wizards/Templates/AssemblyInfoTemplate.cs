using System;

namespace Doji.PackageAuthoring.Editor.Wizards.Templates {
    /// <summary>
    /// Builds assembly metadata for the generated runtime assembly.
    /// </summary>
    public static class AssemblyInfoTemplate {
        public static string GetAssemblyInfo(PackageContext ctx) {
            string copyrightHolder = GetCopyrightHolder(ctx);

            return $@"using System.Reflection;

[assembly: AssemblyTitle(""{ctx.Package.AssemblyName}"")]
[assembly: AssemblyDescription(""{ctx.Package.Description}"")]
[assembly: AssemblyCompany(""{ctx.Project.CompanyName}"")]
[assembly: AssemblyProduct(""{ctx.Package.AssemblyName}"")]
[assembly: AssemblyCopyright(""Copyright (c) {copyrightHolder} {DateTime.Now.Year}"")]

[assembly: AssemblyVersion(""{ctx.Project.Version}.0"")]
[assembly: AssemblyFileVersion(""{ctx.Project.Version}.0"")]
[assembly: AssemblyInformationalVersion(""{ctx.Project.Version}"")]
";
        }

        /// <summary>
        /// Name used in generated copyright notices.
        /// </summary>
        private static string GetCopyrightHolder(PackageContext ctx) {
            return ctx.Repo.CopyrightHolder;
        }
    }
}
