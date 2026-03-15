using System;
using System.Collections.Generic;
using System.IO;

namespace Doji.PackageAuthoring.Editor.Wizards.PackageSearch {
    /// <summary>
    /// Shared in-memory cache of package metadata merged across Unity and project scoped registries.
    /// </summary>
    public sealed class PackageSearchCache : IDisposable {
        private readonly List<IPackageSearchSource> _sources = new();
        private readonly List<PackageSearchEntry> _entries = new();

        public static PackageSearchCache Shared { get; } = new();

        public event Action Changed;

        public bool IsLoading {
            get {
                foreach (var source in _sources) {
                    if (source.IsLoading) {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool HasPackages => _entries.Count > 0;

        public string StatusMessage {
            get {
                if (IsLoading) {
                    return "Loading package indexes...";
                }

                int sourceCount = _sources.Count;
                return sourceCount == 0
                    ? "No package sources configured."
                    : $"Loaded {_entries.Count} packages from {sourceCount} source{(sourceCount == 1 ? string.Empty : "s")}.";
            }
        }

        private PackageSearchCache() {
        }

        public void EnsureLoaded() {
            if (_sources.Count == 0) {
                Refresh();
                return;
            }

            if (!HasPackages && !IsLoading) {
                Refresh();
            }
        }

        public void Refresh() {
            DisposeSources();
            BuildSources();

            foreach (var source in _sources) {
                source.Refresh();
            }

            RebuildEntries();
            Changed?.Invoke();
        }

        public PackageSearchEntry? FindExact(string packageName) {
            foreach (var entry in _entries) {
                if (string.Equals(entry.PackageName, packageName, StringComparison.OrdinalIgnoreCase)) {
                    return entry;
                }
            }

            return null;
        }

        public List<PackageSearchEntry> GetMatches(string query, int maxResults) {
            var matches = new List<PackageSearchEntry>();
            string trimmedQuery = query?.Trim();

            foreach (var entry in _entries) {
                if (!PackageMatchesQuery(entry, trimmedQuery)) {
                    continue;
                }

                matches.Add(entry);
                if (matches.Count >= maxResults) {
                    break;
                }
            }

            return matches;
        }

        public bool HasMoreMatches(string query, int currentMatchCount) {
            string trimmedQuery = query?.Trim();
            int matchCount = 0;

            foreach (var entry in _entries) {
                if (!PackageMatchesQuery(entry, trimmedQuery)) {
                    continue;
                }

                matchCount++;
                if (matchCount > currentMatchCount) {
                    return true;
                }
            }

            return false;
        }

        public void Dispose() {
            DisposeSources();
        }

        private void BuildSources() {
            foreach (var registry in ScopedRegistryManifestReader.ReadFromProjectManifest(Path.Combine("Packages",
                         "manifest.json"))) {
                var source = new ScopedRegistryPackageSearchSource(registry);
                source.Changed += HandleSourceChanged;
                _sources.Add(source);
            }

            var unitySource = new UnityPackageSearchSource();
            unitySource.Changed += HandleSourceChanged;
            _sources.Add(unitySource);
        }

        private void DisposeSources() {
            foreach (var source in _sources) {
                source.Changed -= HandleSourceChanged;
                source.Dispose();
            }

            _sources.Clear();
            _entries.Clear();
        }

        private void HandleSourceChanged() {
            RebuildEntries();
            Changed?.Invoke();
        }

        private void RebuildEntries() {
            _entries.Clear();
            var seenPackageNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var source in _sources) {
                foreach (var entry in source.Entries) {
                    if (!seenPackageNames.Add(entry.PackageName)) {
                        continue;
                    }

                    _entries.Add(entry);
                }
            }

            _entries.Sort((left, right) =>
                string.Compare(left.PackageName, right.PackageName, StringComparison.OrdinalIgnoreCase));
        }

        private static bool PackageMatchesQuery(PackageSearchEntry entry, string query) {
            if (string.IsNullOrWhiteSpace(query)) {
                return true;
            }

            return ContainsIgnoreCase(entry.PackageName, query)
                   || ContainsIgnoreCase(entry.DisplayName, query)
                   || ContainsIgnoreCase(entry.Description, query)
                   || ContainsIgnoreCase(entry.SourceName, query);
        }

        private static bool ContainsIgnoreCase(string value, string query) {
            return !string.IsNullOrEmpty(value)
                   && value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
