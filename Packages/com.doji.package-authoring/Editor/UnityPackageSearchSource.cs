using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageManagerPackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Doji.PackageAuthoring.Editor {
    internal sealed class UnityPackageSearchSource : IPackageSearchSource {
        private readonly List<PackageSearchEntry> _entries = new();

        private SearchRequest _searchRequest;

        public event Action Changed;

        public bool IsLoading => _searchRequest != null;
        public string StatusMessage { get; private set; } = "Loading Unity registry packages...";
        public IReadOnlyList<PackageSearchEntry> Entries => _entries;

        public void Refresh() {
            if (IsLoading) {
                return;
            }

            StatusMessage = "Loading Unity registry packages...";
            _searchRequest = Client.SearchAll();
            EditorApplication.update += PollSearchRequest;
            Changed?.Invoke();
        }

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
