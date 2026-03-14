using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using static Doji.PackageAuthoring.Editor.JsonBuilder;

namespace Doji.PackageAuthoring.Editor {
    public partial class PackageCreationWizard {
        public string GetPackageManifest() {
            var json = Obj(
                Prop("name", packageName),
                Prop("version", version),
                Prop("displayName", productName),
                Prop("description", description),
                Prop("author", Obj(
                    Prop("name", "Doji Technologies"),
                    Prop("url", "https://www.doji-tech.com/"),
                    Prop("email", "support@doji-tech.com")
                )),
                Prop("documentationUrl", $"https://docs.doji-tech.com/{packageName}/"),
                PropIf(createSamplesFolder, "samples", GetSamples()),
                PropIf(dependencies.Count > 0, "dependencies", GetDependencies())
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
                    Prop("description", $"Basic example on how to use {productName}."),
                    Prop("path", "Samples~/01-BasicSample")
                )
            );
        }

        JObject GetDependencies() {
            var obj = new JObject();

            foreach (var dep in dependencies.OrderBy(d => d.packageName))
                obj[dep.packageName] = dep.version;

            return obj;
        }
    }
}
