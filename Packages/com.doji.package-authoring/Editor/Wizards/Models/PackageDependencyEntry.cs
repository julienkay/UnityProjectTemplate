using System;
using UnityEngine;

namespace Doji.PackageAuthoring.Editor.Wizards.Models {
    /// <summary>
    /// Serializable dependency entry for generated package manifests and presets.
    /// </summary>
    [Serializable]
    public class PackageDependencyEntry {
        /// <summary>
        /// Dependency package identifier.
        /// </summary>
        [field: SerializeField]
        public string PackageName { get; set; }

        /// <summary>
        /// Dependency version written into the package manifest.
        /// </summary>
        [field: SerializeField]
        public string Version { get; set; }

        /// <summary>
        /// Copies the package name and version from another dependency entry.
        /// </summary>
        /// <param name="other">The source dependency entry.</param>
        public void CopyFrom(PackageDependencyEntry other) {
            if (other == null) {
                return;
            }

            PackageName = other.PackageName;
            Version = other.Version;
        }
    }
}
