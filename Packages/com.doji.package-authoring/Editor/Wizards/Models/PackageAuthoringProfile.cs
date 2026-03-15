using UnityEngine;

namespace Doji.PackageAuthoring.Editor.Wizards.Models {
    /// <summary>
    /// Shared authoring state used by presets, project defaults, and wizard session data.
    /// </summary>
    [CreateAssetMenu(menuName = "Doji/Package Authoring Preset", fileName = "PackageAuthoringPreset")]
    public class PackageAuthoringProfile : ScriptableObject {
        /// <summary>
        /// Shared project defaults used by both the standalone and companion project flows.
        /// </summary>
        [field: SerializeField]
        public ProjectSettings ProjectDefaults { get; set; } = new();

        /// <summary>
        /// Package defaults used by the package scaffolding flow.
        /// </summary>
        [field: SerializeField]
        public PackageSettings PackageDefaults { get; set; } = new();

        /// <summary>
        /// Repository defaults used by the package scaffolding flow.
        /// </summary>
        [field: SerializeField]
        public RepoSettings RepoDefaults { get; set; } = new();

        /// <summary>
        /// Copies all authoring defaults from another profile.
        /// </summary>
        public void CopyFrom(PackageAuthoringProfile other) {
            if (other == null) {
                return;
            }

            ProjectDefaults ??= new ProjectSettings();
            PackageDefaults ??= new PackageSettings();
            RepoDefaults ??= new RepoSettings();

            ProjectDefaults.CopyFrom(other.ProjectDefaults);
            PackageDefaults.CopyFrom(other.PackageDefaults);
            RepoDefaults.CopyFrom(other.RepoDefaults);
        }
    }
}
