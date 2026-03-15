namespace Doji.PackageAuthoring.Editor.Wizards.Templates {
    /// <summary>
    /// Builds starter scripts included in generated samples.
    /// </summary>
    public static class ScriptTemplates {
        public static string GetSampleScript(PackageContext ctx) {
            return $@"using UnityEngine;

namespace {ctx.Package.NamespaceName}.Samples {{

    public class {ctx.Project.ProductName.Replace(" ", string.Empty)}_BasicSample : MonoBehaviour {{

    }}
}}";
        }
    }
}
