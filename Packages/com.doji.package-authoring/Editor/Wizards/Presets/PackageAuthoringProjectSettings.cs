using UnityEditor;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;

namespace Doji.PackageAuthoring.Editor.Wizards.Presets {
    /// <summary>
    /// Project-scoped persistent defaults used when the package authoring tools are opened without applying a preset.
    /// </summary>
    [FilePath("ProjectSettings/PackageAuthoringProjectSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class PackageAuthoringProjectSettings : ScriptableSingleton<PackageAuthoringProjectSettings> {
        [SerializeField] private ProjectSettings projectDefaults = new() {
            ProductName = "MyPackage"
        };

        [SerializeField] private PackageSettings packageDefaults = new();
        [SerializeField] private RepoSettings repoDefaults = new();

        /// <summary>
        /// Shared project defaults that seed both creation wizards.
        /// </summary>
        public ProjectSettings ProjectDefaults => projectDefaults;

        /// <summary>
        /// Package defaults used by the package creation wizard and settings provider.
        /// </summary>
        public PackageSettings PackageDefaults => packageDefaults;

        /// <summary>
        /// Repository defaults used by the package creation wizard and repository root templates.
        /// </summary>
        public RepoSettings RepoDefaults => repoDefaults;

        /// <summary>
        /// Saves the current settings instance back into the project settings asset.
        /// </summary>
        public void SaveSettings() {
            Save(true);
        }
    }
}
