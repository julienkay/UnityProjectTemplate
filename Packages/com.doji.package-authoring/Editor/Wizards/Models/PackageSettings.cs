using System;
using System.Collections.Generic;
using UnityEngine;

namespace Doji.PackageAuthoring.Editor.Wizards.Models {
    /// <summary>
    /// Shared package-facing settings used by package scaffolding presets and the package wizard.
    /// </summary>
    [Serializable]
    public class PackageSettings {
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
        /// Company name written into generated package metadata such as the manifest author block.
        /// </summary>
        [field: SerializeField]
        public string CompanyName { get; set; } = "Your Company";

        /// <summary>
        /// Whether author metadata should be emitted into generated package files.
        /// </summary>
        [field: SerializeField]
        public bool IncludeAuthor { get; set; } = true;

        /// <summary>
        /// Optional author URL written into the generated package manifest.
        /// </summary>
        [field: SerializeField]
        public string AuthorUrl { get; set; } = string.Empty;

        /// <summary>
        /// Optional author email written into the generated package manifest.
        /// </summary>
        [field: SerializeField]
        public string AuthorEmail { get; set; } = string.Empty;

        /// <summary>
        /// Whether the generated package manifest should include a minimum Unity version.
        /// </summary>
        [field: SerializeField]
        public bool IncludeMinimumUnityVersion { get; set; }

        /// <summary>
        /// Major Unity version written into the package manifest when enabled.
        /// </summary>
        [field: SerializeField]
        public string MinimumUnityMajor { get; set; } = string.Empty;

        /// <summary>
        /// Minor Unity version written into the package manifest when enabled.
        /// </summary>
        [field: SerializeField]
        public string MinimumUnityMinor { get; set; } = string.Empty;

        /// <summary>
        /// Optional Unity release suffix written into the package manifest when enabled.
        /// </summary>
        [field: SerializeField]
        public string MinimumUnityRelease { get; set; } = string.Empty;

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
        public List<PackageDependencyEntry> Dependencies { get; set; } = new();

        /// <summary>
        /// Copies all package-facing values from another settings instance, including dependency entries.
        /// </summary>
        /// <param name="other">The source settings to copy from.</param>
        public void CopyFrom(PackageSettings other) {
            if (other == null) {
                return;
            }

            PackageName = other.PackageName;
            AssemblyName = other.AssemblyName;
            NamespaceName = other.NamespaceName;
            Description = other.Description;
            CompanyName = other.CompanyName;
            IncludeAuthor = other.IncludeAuthor;
            AuthorUrl = other.AuthorUrl;
            AuthorEmail = other.AuthorEmail;
            IncludeMinimumUnityVersion = other.IncludeMinimumUnityVersion;
            MinimumUnityMajor = other.MinimumUnityMajor;
            MinimumUnityMinor = other.MinimumUnityMinor;
            MinimumUnityRelease = other.MinimumUnityRelease;
            CreateDocsFolder = other.CreateDocsFolder;
            CreateSamplesFolder = other.CreateSamplesFolder;
            CreateEditorFolder = other.CreateEditorFolder;
            CreateTestsFolder = other.CreateTestsFolder;

            Dependencies = new List<PackageDependencyEntry>(other.Dependencies?.Count ?? 0);
            if (other.Dependencies == null) {
                return;
            }

            foreach (var dependency in other.Dependencies) {
                var clone = new PackageDependencyEntry();
                clone.CopyFrom(dependency);
                Dependencies.Add(clone);
            }
        }
    }
}
