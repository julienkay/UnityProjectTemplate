public partial class PackageCreationWizard {
    public string GetSampleScript() {
        return $@"using UnityEngine;

namespace {namespaceName}.Samples {{

    public class {productName}_BasicSample : MonoBehaviour {{

    }}
}}";
    }
}
