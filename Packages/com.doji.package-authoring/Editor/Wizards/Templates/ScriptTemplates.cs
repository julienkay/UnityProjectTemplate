namespace Doji.PackageAuthoring.Editor.Wizards {
    public partial class PackageCreationWizard {
        public string GetSampleScript() {
            return $@"using UnityEngine;

namespace {_packageSettings.NamespaceName}.Samples {{

    public class {_projectSettings.ProductName.Replace(" ", string.Empty)}_BasicSample : MonoBehaviour {{

    }}
}}";
        }
    }
}
