using UnityEngine;

namespace Doji.PackageAuthoring.Editor {
    /// <summary>
    /// Reusable preset asset for package authoring and project scaffolding workflows.
    /// </summary>
    [CreateAssetMenu(menuName = "Doji/Package Authoring Preset", fileName = "PackageAuthoringPreset")]
    public sealed class PackageAuthoringDefaults : ScriptableObject {
        [SerializeField] private ProjectScaffoldSettings projectDefaults = new() {
            ProductName = "MyPackage"
        };
        [SerializeField] private PackageScaffoldSettings packageDefaults = new();

        /// <summary>
        /// Shared project defaults used by both the standalone and companion project flows.
        /// </summary>
        public ProjectScaffoldSettings ProjectDefaults => projectDefaults;

        /// <summary>
        /// Package defaults used by the package scaffolding flow.
        /// </summary>
        public PackageScaffoldSettings PackageDefaults => packageDefaults;
    }
}
