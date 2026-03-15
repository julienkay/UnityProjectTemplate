namespace Doji.PackageAuthoring.Editor.Wizards.PackageSearch {
    /// <summary>
    /// Normalized package metadata consumed by editor autocomplete UI regardless of package source.
    /// This keeps package identity, the latest selectable version, and human-facing labels in one shape whether
    /// the entry came from Unity's package manager API or a scoped npm-compatible registry.
    /// </summary>
    public readonly struct PackageSearchEntry {
        /// <summary>
        /// Canonical package identifier used in manifests and dependency declarations, for example
        /// <c>com.unity.addressables</c>.
        /// </summary>
        public string PackageName { get; }

        /// <summary>
        /// Latest version currently exposed by the source for quick insertion into dependency fields,
        /// for example <c>2.3.16</c>.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Human-facing package label shown by the registry when available, for example <c>Addressables</c>.
        /// When a source does not provide one, this falls back to <see cref="PackageName"/>.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Longer package summary text returned by the source, for example a sentence from the package manifest.
        /// This is retained for UI and future filtering even when autocomplete does not match on prose text.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Search-oriented tags declared by the package manifest, for example <c>testing</c>, <c>editor</c>,
        /// or <c>ci</c>.
        /// </summary>
        public string[] Keywords { get; }

        /// <summary>
        /// Friendly label for the registry or provider that supplied the package, for example <c>Unity</c> or
        /// a scoped registry name from <c>Packages/manifest.json</c>.
        /// </summary>
        public string SourceName { get; }

        /// <param name="packageName">
        /// Canonical dependency id such as <c>com.unity.test-framework</c>.
        /// </param>
        /// <param name="version">
        /// Latest version string exposed by the source, such as <c>1.4.5</c>.
        /// </param>
        /// <param name="displayName">
        /// Human-facing label shown to users, such as <c>Test Framework</c>.
        /// </param>
        /// <param name="description">
        /// Source-provided summary text for the package.
        /// </param>
        /// <param name="keywords">
        /// Manifest keywords used for categorical search, such as <c>testing</c> or <c>editor</c>.
        /// </param>
        /// <param name="sourceName">
        /// Friendly registry label such as <c>Unity</c> or <c>OpenUPM</c>.
        /// </param>
        public PackageSearchEntry(
            string packageName,
            string version,
            string displayName,
            string description,
            string[] keywords,
            string sourceName) {
            PackageName = packageName;
            Version = version;
            DisplayName = displayName;
            Description = description;
            Keywords = keywords ?? System.Array.Empty<string>();
            SourceName = sourceName;
        }
    }
}
