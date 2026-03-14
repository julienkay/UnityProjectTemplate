using System;
using System.Collections.Generic;
using UnityEngine;

namespace Doji.PackageAuthoring.Editor {
    /// <summary>
    /// Shared package-facing settings used by package scaffolding presets and the package wizard.
    /// </summary>
    [Serializable]
    public class PackageScaffoldSettings {
        /// <summary>
        /// Package identifier written to the generated <c>package.json</c>.
        /// </summary>
        [field: SerializeField]
        public string PackageName { get; set; } = "com.yourcompany.yourpackage";

        /// <summary>
        /// Primary runtime assembly name used for generated asmdef files.
        /// </summary>
        [field: SerializeField]
        public string AssemblyName { get; set; } = "YourCompany.YourPackageAssembly";

        /// <summary>
        /// Default namespace used by generated scripts and documentation.
        /// </summary>
        [field: SerializeField]
        public string NamespaceName { get; set; } = "YourCompany.YourPackageNamespace";

        /// <summary>
        /// Short package description shown in generated metadata and documentation.
        /// </summary>
        [field: SerializeField]
        public string Description { get; set; } = "Describe what this package provides.";

        /// <summary>
        /// Author name written into generated package files such as the license.
        /// </summary>
        [field: SerializeField]
        public string Author { get; set; } = "Your Name";

        /// <summary>
        /// License template used for generated package files.
        /// </summary>
        [field: SerializeField]
        public LicenseType LicenseType { get; set; } = LicenseType.MIT;

        /// <summary>
        /// Whether to include a documentation folder scaffold.
        /// </summary>
        [field: SerializeField]
        public bool CreateDocsFolder { get; set; } = true;

        /// <summary>
        /// Whether to include a <c>Samples~</c> folder.
        /// </summary>
        [field: SerializeField]
        public bool CreateSamplesFolder { get; set; }

        /// <summary>
        /// Whether to include an editor-only assembly and folder.
        /// </summary>
        [field: SerializeField]
        public bool CreateEditorFolder { get; set; }

        /// <summary>
        /// Whether to include a tests folder scaffold.
        /// </summary>
        [field: SerializeField]
        public bool CreateTestsFolder { get; set; }

        /// <summary>
        /// Gets the dependency entries written into the generated package manifest.
        /// </summary>
        [field: SerializeField]
        public List<PackageDependencyEntry> Dependencies { get; set; } = new() {
            new() {
                PackageName = "com.unity.ai.inference",
                Version = "2.3.0"
            }
        };

        /// <summary>
        /// Copies all package-facing values from another settings instance, including dependency entries.
        /// </summary>
        /// <param name="other">The source settings to copy from.</param>
        public void CopyFrom(PackageScaffoldSettings other) {
            if (other == null) {
                return;
            }

            PackageName = other.PackageName;
            AssemblyName = other.AssemblyName;
            NamespaceName = other.NamespaceName;
            Description = other.Description;
            Author = other.Author;
            LicenseType = other.LicenseType;
            CreateDocsFolder = other.CreateDocsFolder;
            CreateSamplesFolder = other.CreateSamplesFolder;
            CreateEditorFolder = other.CreateEditorFolder;
            CreateTestsFolder = other.CreateTestsFolder;

            Dependencies = new List<PackageDependencyEntry>(other.Dependencies.Count);
            foreach (var dependency in other.Dependencies) {
                var clone = new PackageDependencyEntry();
                clone.CopyFrom(dependency);
                Dependencies.Add(clone);
            }
        }
    }
}
