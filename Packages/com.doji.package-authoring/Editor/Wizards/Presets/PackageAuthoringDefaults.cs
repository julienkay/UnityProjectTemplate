using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;

namespace Doji.PackageAuthoring.Editor.Wizards.Presets {
    /// <summary>
    /// Reusable preset asset for package authoring and project scaffolding workflows.
    /// </summary>
    [CreateAssetMenu(menuName = "Doji/Package Authoring Preset", fileName = "PackageAuthoringPreset")]
    public sealed class PackageAuthoringDefaults : ScriptableObject {
        [SerializeField] private ProjectSettings projectDefaults = new() {
            ProductName = "MyPackage"
        };

        [SerializeField] private PackageSettings packageDefaults = new();
        [SerializeField] private RepoSettings repoDefaults = new();

        /// <summary>
        /// Shared project defaults used by both the standalone and companion project flows.
        /// </summary>
        public ProjectSettings ProjectDefaults => projectDefaults;

        /// <summary>
        /// Package defaults used by the package scaffolding flow.
        /// </summary>
        public PackageSettings PackageDefaults => packageDefaults;

        /// <summary>
        /// Repository defaults used by the package scaffolding flow.
        /// </summary>
        public RepoSettings RepoDefaults => repoDefaults;
    }
}
