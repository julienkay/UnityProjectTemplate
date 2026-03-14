using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Doji.PackageAuthoring.Editor.Wizards.PackageSearch {
    internal static class ScopedRegistryManifestReader {
        internal readonly struct ScopedRegistryDefinition {
            public readonly string Name;
            public readonly string Url;
            public readonly IReadOnlyList<string> Scopes;

            public ScopedRegistryDefinition(string name, string url, IReadOnlyList<string> scopes) {
                Name = name;
                Url = url;
                Scopes = scopes;
            }
        }

        public static List<ScopedRegistryDefinition> ReadFromProjectManifest(string manifestPath) {
            var registries = new List<ScopedRegistryDefinition>();
            if (!File.Exists(manifestPath)) {
                return registries;
            }

            var root = JObject.Parse(File.ReadAllText(manifestPath));
            if (root["scopedRegistries"] is not JArray scopedRegistriesToken) {
                return registries;
            }

            foreach (var registryToken in scopedRegistriesToken) {
                string name = registryToken.Value<string>("name");
                string url = registryToken.Value<string>("url");
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(url)) {
                    continue;
                }

                var scopes = new List<string>();
                if (registryToken["scopes"] is JArray scopesToken) {
                    foreach (var scopeToken in scopesToken) {
                        string scope = scopeToken.Value<string>();
                        if (!string.IsNullOrWhiteSpace(scope)) {
                            scopes.Add(scope);
                        }
                    }
                }

                registries.Add(new ScopedRegistryDefinition(name, url, scopes));
            }

            return registries;
        }
    }
}
