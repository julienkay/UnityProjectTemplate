using System;
using UnityEngine;

namespace Doji.PackageAuthoring.Editor.Wizards.Models {
    /// <summary>
    /// Shared project-facing settings used by both the standalone and companion project wizards.
    /// </summary>
    [Serializable]
    public class ProjectScaffoldSettings {
        /// <summary>
        /// Company or organization name written into generated Unity project settings.
        /// </summary>
        [field: SerializeField]
        public string CompanyName { get; set; } = "Your Company";

        /// <summary>
        /// User-facing project name for generated standalone or companion projects.
        /// </summary>
        [field: SerializeField]
        public string ProductName { get; set; } = "My Project";

        /// <summary>
        /// Version applied to generated project and package metadata.
        /// </summary>
        [field: SerializeField]
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Base output folder used when scaffolding projects from the editor windows.
        /// </summary>
        [field: SerializeField]
        public string TargetLocation { get; set; } = "../";

        /// <summary>
        /// Copies all project-facing values from another settings instance.
        /// </summary>
        /// <param name="other">The source settings to copy from.</param>
        public void CopyFrom(ProjectScaffoldSettings other) {
            if (other == null) {
                return;
            }

            CompanyName = other.CompanyName;
            ProductName = other.ProductName;
            Version = other.Version;
            TargetLocation = other.TargetLocation;
        }
    }
}
