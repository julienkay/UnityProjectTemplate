using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine.Networking;

namespace Doji.PackageAuthoring.Editor.Wizards.PackageSearch {
    internal sealed class ScopedRegistryPackageSearchSource : IPackageSearchSource {
        private readonly ScopedRegistryManifestReader.ScopedRegistryDefinition _registry;
        private readonly List<PackageSearchEntry> _entries = new();

        private UnityWebRequest _request;
        private UnityWebRequestAsyncOperation _requestOperation;

        public event Action Changed;

        public bool IsLoading => _requestOperation != null;
        public string StatusMessage { get; private set; }
        public IReadOnlyList<PackageSearchEntry> Entries => _entries;

        public ScopedRegistryPackageSearchSource(ScopedRegistryManifestReader.ScopedRegistryDefinition registry) {
            _registry = registry;
            StatusMessage = $"Loading {_registry.Name} registry packages...";
        }

        public void Refresh() {
            if (IsLoading) {
                return;
            }

            _entries.Clear();
            StatusMessage = $"Loading {_registry.Name} registry packages...";
            string requestUrl = $"{_registry.Url.TrimEnd('/')}/-/all";
            _request = UnityWebRequest.Get(requestUrl);
            _requestOperation = _request.SendWebRequest();
            EditorApplication.update += PollRequest;
            Changed?.Invoke();
        }

        public void Dispose() {
            EditorApplication.update -= PollRequest;
            _request?.Dispose();
            _request = null;
            _requestOperation = null;
        }

        private void PollRequest() {
            if (_requestOperation == null || !_requestOperation.isDone) {
                return;
            }

            EditorApplication.update -= PollRequest;

            if (_request.result == UnityWebRequest.Result.Success) {
                ParseRegistryResponse(_request.downloadHandler.text);
                _entries.Sort((left, right) =>
                    string.Compare(left.PackageName, right.PackageName, StringComparison.OrdinalIgnoreCase));
                StatusMessage = $"Loaded {_entries.Count} packages from {_registry.Name}.";
            }
            else {
                StatusMessage = $"Failed to load {_registry.Name}: {_request.error}";
            }

            _request.Dispose();
            _request = null;
            _requestOperation = null;
            Changed?.Invoke();
        }

        private void ParseRegistryResponse(string json) {
            var root = JObject.Parse(json);
            foreach (var property in root.Properties()) {
                string packageName = property.Name;
                if (!MatchesRegistryScopes(packageName)) {
                    continue;
                }

                var packageToken = (JObject)property.Value;
                string latestVersion = packageToken.SelectToken("dist-tags.latest")?.Value<string>();
                if (string.IsNullOrWhiteSpace(latestVersion)) {
                    continue;
                }

                string displayName = packageToken.SelectToken($"versions['{latestVersion}'].displayName")
                    ?.Value<string>();
                string description =
                    packageToken.SelectToken($"versions['{latestVersion}'].description")?.Value<string>()
                    ?? packageToken.Value<string>("description");

                _entries.Add(new PackageSearchEntry(
                    packageName,
                    latestVersion,
                    string.IsNullOrWhiteSpace(displayName) ? packageName : displayName,
                    description,
                    _registry.Name));
            }
        }

        private bool MatchesRegistryScopes(string packageName) {
            if (_registry.Scopes == null || _registry.Scopes.Count == 0) {
                return true;
            }

            foreach (var scope in _registry.Scopes) {
                if (packageName.StartsWith(scope, StringComparison.OrdinalIgnoreCase)) {
                    return true;
                }
            }

            return false;
        }
    }
}
