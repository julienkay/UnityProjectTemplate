using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageManagerPackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Doji.PackageAuthoring.Editor.Wizards.PackageSearch {
    /// <summary>
    /// Queries the Unity package manager for packages available from the configured Unity registry sources.
    /// This is kept separate from scoped registry lookup because Unity exposes it through the package manager API.
    /// </summary>
    internal sealed class UnityPackageSearchSource : IPackageSearchSource {
        private readonly List<PackageSearchEntry> _entries = new();

        private SearchRequest _searchRequest;

        public event Action Changed;

        public bool IsLoading => _searchRequest != null;
        public string StatusMessage { get; private set; } = "Loading Unity registry packages...";
        public IReadOnlyList<PackageSearchEntry> Entries => _entries;

        /// <summary>
        /// Starts a package-manager search unless a previous request is still pending.
        /// </summary>
        public void Refresh() {
            if (IsLoading) {
                return;
            }

            StatusMessage = "Loading Unity registry packages...";
            _searchRequest = Client.SearchAll();
            EditorApplication.update += PollSearchRequest;
            Changed?.Invoke();
        }

        /// <summary>
        /// Unhooks the editor polling callback used to observe the asynchronous search request.
        /// </summary>
        public void Dispose() {
            EditorApplication.update -= PollSearchRequest;
        }

        private void PollSearchRequest() {
            if (_searchRequest == null || !_searchRequest.IsCompleted) {
                return;
            }

            EditorApplication.update -= PollSearchRequest;
            _entries.Clear();

            if (_searchRequest.Status == StatusCode.Success) {
                foreach (PackageManagerPackageInfo packageInfo in _searchRequest.Result) {
                    if (packageInfo == null) {
                        continue;
                    }

                    _entries.Add(new PackageSearchEntry(
                        packageInfo.name,
                        packageInfo.version,
                        string.IsNullOrWhiteSpace(packageInfo.displayName) ? packageInfo.name : packageInfo.displayName,
                        packageInfo.description,
                        packageInfo.keywords,
                        "Unity"));
                }

                _entries.Sort((left, right) =>
                    string.Compare(left.PackageName, right.PackageName, StringComparison.OrdinalIgnoreCase));
                StatusMessage = $"Loaded {_entries.Count} Unity registry packages.";
            }
            else {
                StatusMessage = _searchRequest.Error?.message ?? "Failed to load Unity registry packages.";
            }

            _searchRequest = null;
            Changed?.Invoke();
        }
    }
}
