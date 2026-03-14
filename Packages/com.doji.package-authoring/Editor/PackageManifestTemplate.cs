using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using static Doji.PackageAuthoring.Editor.JsonBuilder;

namespace Doji.PackageAuthoring.Editor {
    public partial class PackageCreationWizard {
        public string GetPackageManifest() {
            var json = Obj(
                Prop("name", _packageName),
                Prop("version", _version),
                Prop("displayName", _productName),
                Prop("description", _description),
                Prop("author", Obj(
                    Prop("name", "Doji Technologies"),
                    Prop("url", "https://www.doji-tech.com/"),
                    Prop("email", "support@doji-tech.com")
                )),
                Prop("documentationUrl", $"https://docs.doji-tech.com/{_packageName}/"),
                PropIf(createSamplesFolder, "samples", GetSamples()),
                PropIf(_dependencies.Count > 0, "dependencies", GetDependencies())
            );

            return json.ToString(Formatting.Indented);
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
                    Prop("description", $"Basic example on how to use {_productName}."),
                    Prop("path", "Samples~/01-BasicSample")
                )
            );
        }

        JObject GetDependencies() {
            var obj = new JObject();

            foreach (var dep in _dependencies.OrderBy(d => d.PackageName)) {
                obj[dep.PackageName] = dep.Version;
            }

            return obj;
        }
    }
}
