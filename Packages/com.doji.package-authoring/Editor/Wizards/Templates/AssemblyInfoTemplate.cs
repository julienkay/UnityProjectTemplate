using System;

namespace Doji.PackageAuthoring.Editor.Wizards {
    public partial class PackageCreationWizard {
        public string GetAssemblyInfo() {
            string copyrightHolder = GetCopyrightHolder();

            return $@"using System.Reflection;

[assembly: AssemblyTitle(""{_packageSettings.AssemblyName}"")]
[assembly: AssemblyDescription(""{_packageSettings.Description}"")]
[assembly: AssemblyCompany(""{_projectSettings.CompanyName}"")]
[assembly: AssemblyProduct(""{_packageSettings.AssemblyName}"")]
[assembly: AssemblyCopyright(""Copyright (c) {copyrightHolder} {DateTime.Now.Year}"")]

[assembly: AssemblyVersion(""{_projectSettings.Version}.0"")]
[assembly: AssemblyFileVersion(""{_projectSettings.Version}.0"")]
[assembly: AssemblyInformationalVersion(""{_projectSettings.Version}"")]
";
        }

        /// <summary>
        /// Name used in generated copyright notices.
        /// </summary>
        private string GetCopyrightHolder() {
            return _repoSettings.CopyrightHolder;
        }
    }
}
