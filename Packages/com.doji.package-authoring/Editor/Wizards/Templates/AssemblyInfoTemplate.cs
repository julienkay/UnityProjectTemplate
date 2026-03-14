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
        /// Falls back to the project company when author metadata is disabled or empty.
        /// </summary>
        private string GetCopyrightHolder() {
            return _packageSettings.IncludeAuthor && !string.IsNullOrWhiteSpace(_packageSettings.Author)
                ? _packageSettings.Author
                : _projectSettings.CompanyName;
        }
    }
}
