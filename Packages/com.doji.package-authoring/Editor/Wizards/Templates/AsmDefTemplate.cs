namespace Doji.PackageAuthoring.Editor.Wizards {
    public partial class PackageCreationWizard {
        /// <summary>
        /// Builds the runtime asmdef JSON for the generated package.
        /// </summary>
        public string GetRuntimeAsmDef() {
            return $@"{{
    ""name"": ""{_packageSettings.AssemblyName}"",
    ""rootNamespace"": ""{_packageSettings.NamespaceName}"",
    ""references"": [],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}}";
        }

        /// <summary>
        /// Builds the samples asmdef JSON for the generated package.
        /// </summary>
        /// <param name="runtimeAssemblyGuid">GUID of the generated runtime asmdef used for stable references.</param>
        public string GetSamplesAsmDef(string runtimeAssemblyGuid) {
            return $@"{{
    ""name"": ""{_packageSettings.AssemblyName}.Samples"",
    ""rootNamespace"": ""{_packageSettings.NamespaceName}.Samples"",
    ""references"": [
        ""GUID:{runtimeAssemblyGuid}""
    ],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}}";
        }

        /// <summary>
        /// Builds the editor-only asmdef JSON for the generated package.
        /// </summary>
        /// <param name="runtimeAssemblyGuid">GUID of the generated runtime asmdef used for stable references.</param>
        public string GetEditorAsmDef(string runtimeAssemblyGuid) {
            return $@"{{
    ""name"": ""{_packageSettings.AssemblyName}.Editor"",
    ""rootNamespace"": ""{_packageSettings.NamespaceName}.Editor"",
    ""references"": [
        ""GUID:{runtimeAssemblyGuid}""
    ],
    ""includePlatforms"": [
        ""Editor""
    ],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}}";
        }
    }
}
