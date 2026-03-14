namespace Doji.PackageAuthoring.Editor {
    public partial class PackageCreationWizard {
        public string GetSampleScript() {
            return $@"using UnityEngine;

namespace {_namespaceName}.Samples {{

    public class {_productName.Replace(" ", string.Empty)}_BasicSample : MonoBehaviour {{

    }}
}}";
        }
    }
}
