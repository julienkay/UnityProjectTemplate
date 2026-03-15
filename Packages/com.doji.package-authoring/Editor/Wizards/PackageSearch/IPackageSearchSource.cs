using System;
using System.Collections.Generic;

namespace Doji.PackageAuthoring.Editor.Wizards.PackageSearch {
    /// <summary>
    /// Supplies package search results from one backing source such as the Unity registry or a scoped npm registry.
    /// Implementations may load asynchronously and are expected to raise <see cref="Changed"/> whenever their status or entries change.
    /// </summary>
    internal interface IPackageSearchSource : IDisposable {
        /// <summary>
        /// Raised when either loading state, status text, or available entries changed.
        /// </summary>
        event Action Changed;

        /// <summary>
        /// Indicates whether the source currently has an in-flight refresh operation.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Short human-readable status describing the current load state or last result.
        /// </summary>
        string StatusMessage { get; }

        /// <summary>
        /// Latest package entries published by this source.
        /// </summary>
        IReadOnlyList<PackageSearchEntry> Entries { get; }

        /// <summary>
        /// Starts or re-starts loading package entries for this source.
        /// </summary>
        void Refresh();
    }
}
