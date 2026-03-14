using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Doji.PackageAuthoring.Editor.JsonBuilder;

namespace Doji.PackageAuthoring.Editor {

public partial class PackageCreationWizard {
    public string GetProjectManifest() {
        var json = Obj(
            Prop("dependencies", GetProjectDependencies()),
            PropIf(createTestsFolder, "testables", Arr(packageName))
        );

        return json.ToString(Formatting.Indented);
    }

    JObject GetProjectDependencies() {
        var deps = new JObject {
            [packageName] = $"file:../../../{packageName}",
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
