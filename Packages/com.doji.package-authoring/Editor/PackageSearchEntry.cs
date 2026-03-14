namespace Doji.PackageAuthoring.Editor {
    /// <summary>
    /// Normalized package metadata consumed by editor autocomplete UI regardless of package source.
    /// </summary>
    public readonly struct PackageSearchEntry {
        public readonly string PackageName;
        public readonly string Version;
        public readonly string DisplayName;
        public readonly string Description;
        public readonly string SourceName;

        public PackageSearchEntry(
            string packageName,
            string version,
            string displayName,
            string description,
            string sourceName) {
            PackageName = packageName;
            Version = version;
            DisplayName = displayName;
            Description = description;
            SourceName = sourceName;
        }
    }
}
