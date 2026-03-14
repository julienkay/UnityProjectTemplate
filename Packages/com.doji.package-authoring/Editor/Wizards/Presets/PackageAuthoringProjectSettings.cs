using UnityEditor;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;

namespace Doji.PackageAuthoring.Editor.Wizards.Presets {
    /// <summary>
    /// Project-scoped persistent defaults used when the package authoring tools are opened without applying a preset.
    /// </summary>
    [FilePath("ProjectSettings/PackageAuthoringProjectSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class PackageAuthoringProjectSettings : ScriptableSingleton<PackageAuthoringProjectSettings> {
        [SerializeField] private ProjectScaffoldSettings projectDefaults = new() {
            ProductName = "MyPackage"
        };
        [SerializeField] private PackageScaffoldSettings packageDefaults = new();
        [SerializeField] private RepoScaffoldSettings repoDefaults = new();

        /// <summary>
        /// Shared project defaults that seed both creation wizards.
        /// </summary>
        public ProjectScaffoldSettings ProjectDefaults => projectDefaults;

        /// <summary>
        /// Package defaults used by the package creation wizard and settings provider.
        /// </summary>
        public PackageScaffoldSettings PackageDefaults => packageDefaults;

        /// <summary>
        /// Repository defaults used by the package creation wizard and repository root templates.
        /// </summary>
        public RepoScaffoldSettings RepoDefaults => repoDefaults;

        /// <summary>
        /// Saves the current settings instance back into the project settings asset.
        /// </summary>
        public void SaveSettings() {
            Save(true);
        }
    }
}
