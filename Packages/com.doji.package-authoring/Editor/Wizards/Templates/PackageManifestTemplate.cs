using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using static Doji.PackageAuthoring.Editor.Utilities.JsonBuilder;

namespace Doji.PackageAuthoring.Editor.Wizards {
    public partial class PackageCreationWizard {
        public string GetPackageManifest() {
            var json = Obj(
                Prop("name", _packageSettings.PackageName),
                Prop("version", _projectSettings.Version),
                Prop("displayName", _projectSettings.ProductName),
                Prop("description", _packageSettings.Description),
                Prop("author", _packageSettings.IncludeAuthor ? GetAuthorMetadata() : null),
                PropIf(
                    _packageSettings.IncludeMinimumUnityVersion,
                    "unity",
                    $"{_packageSettings.MinimumUnityMajor}.{_packageSettings.MinimumUnityMinor}"),
                PropIf(
                    _packageSettings.IncludeMinimumUnityVersion &&
                    !string.IsNullOrWhiteSpace(_packageSettings.MinimumUnityRelease),
                    "unityRelease",
                    _packageSettings.MinimumUnityRelease),
                Prop("documentationUrl", $"https://docs.doji-tech.com/{_packageSettings.PackageName}/"),
                PropIf(_packageSettings.CreateSamplesFolder, "samples", GetSamples()),
                PropIf(_packageSettings.Dependencies.Count > 0, "dependencies", GetDependencies())
            );

            return json.ToString(Formatting.Indented);
        }

        private JObject GetAuthorMetadata() {
            var author = Obj(
                Prop("name", _packageSettings.CompanyName),
                Prop("url", _packageSettings.AuthorUrl),
                Prop("email", _packageSettings.AuthorEmail)
            );

            return author.HasValues ? author : null;
        }

        JArray GetSamples() {
            return Arr(
                Obj(
                    Prop("displayName", "Shared Sample Assets (Required)"),
                    Prop("description",
                        "Shared resources for samples. All other samples depend on this being imported."),
                    Prop("path", "Samples~/00-SharedSampleAssets")
                ),
                Obj(
                    Prop("displayName", "Basic Sample"),
                    Prop("description", $"Basic example on how to use {_projectSettings.ProductName}."),
                    Prop("path", "Samples~/01-BasicSample")
                )
            );
        }

        JObject GetDependencies() {
            var obj = new JObject();

            foreach (var dep in _packageSettings.Dependencies.OrderBy(d => d.PackageName)) {
                obj[dep.PackageName] = dep.Version;
            }

            return obj;
        }
    }
}
