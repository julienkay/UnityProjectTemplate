using System;
using System.Collections.Generic;
using UnityEngine;

namespace Doji.PackageAuthoring.Editor.Wizards.Models {
    /// <summary>
    /// Serialized dependency collection written into generated package manifests.
    /// </summary>
    [Serializable]
    public class PackageDependencyList {
        /// <summary>
        /// Ordered dependency entries written into the generated package manifest.
        /// </summary>
        [field: SerializeField]
        public List<PackageDependencyEntry> Items { get; set; } = new();

        /// <summary>
        /// Number of dependency entries currently stored in the collection.
        /// </summary>
        public int Count => Items?.Count ?? 0;

        /// <summary>
        /// Copies all dependency entries from another serialized collection.
        /// </summary>
        public void CopyFrom(PackageDependencyList other) {
            Items = new List<PackageDependencyEntry>(other?.Items?.Count ?? 0);
            if (other?.Items == null) {
                return;
            }

            foreach (var dependency in other.Items) {
                var clone = new PackageDependencyEntry();
                clone.CopyFrom(dependency);
                Items.Add(clone);
            }
        }
    }
}
