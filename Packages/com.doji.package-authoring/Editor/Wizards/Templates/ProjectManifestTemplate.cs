using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Doji.PackageAuthoring.Editor.Utilities.JsonBuilder;

namespace Doji.PackageAuthoring.Editor.Wizards.Templates {
    /// <summary>
    /// Builds the companion project's Unity manifest.
    /// </summary>
    public static class ProjectManifestTemplate {
        public static string GetProjectManifest(PackageContext ctx) {
            var json = Obj(
                Prop("dependencies", GetProjectDependencies(ctx)),
                PropIf(ctx.Package.CreateTestsFolder, "testables", Arr(ctx.Package.PackageName))
            );

            return json.ToString(Formatting.Indented);
        }

        private static JObject GetProjectDependencies(PackageContext ctx) {
            var deps = new JObject {
                [ctx.Package.PackageName] = $"file:../../../{ctx.Package.PackageName}",
                ["com.unity.ide.visualstudio"] = "2.0.27",
                ["com.unity.ugui"] = "2.0.0",
                ["com.unity.modules.ai"] = "1.0.0",
                ["com.unity.modules.androidjni"] = "1.0.0",
                ["com.unity.modules.animation"] = "1.0.0",
                ["com.unity.modules.assetbundle"] = "1.0.0",
                ["com.unity.modules.audio"] = "1.0.0",
                ["com.unity.modules.imageconversion"] = "1.0.0",
                ["com.unity.modules.imgui"] = "1.0.0",
                ["com.unity.modules.jsonserialize"] = "1.0.0",
                ["com.unity.modules.particlesystem"] = "1.0.0",
                ["com.unity.modules.physics"] = "1.0.0",
                ["com.unity.modules.physics2d"] = "1.0.0",
                ["com.unity.modules.screencapture"] = "1.0.0",
                ["com.unity.modules.tilemap"] = "1.0.0",
                ["com.unity.modules.ui"] = "1.0.0",
                ["com.unity.modules.uielements"] = "1.0.0",
                ["com.unity.modules.unitywebrequest"] = "1.0.0",
                ["com.unity.modules.unitywebrequestassetbundle"] = "1.0.0",
                ["com.unity.modules.unitywebrequestaudio"] = "1.0.0",
                ["com.unity.modules.unitywebrequesttexture"] = "1.0.0",
                ["com.unity.modules.unitywebrequestwww"] = "1.0.0",
                ["com.unity.modules.video"] = "1.0.0",
                ["com.unity.modules.vr"] = "1.0.0",
                ["com.unity.modules.xr"] = "1.0.0"
            };

            return new JObject(deps.Properties());
        }
    }
}
