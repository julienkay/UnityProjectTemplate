namespace Doji.PackageAuthoring.Editor {
    public partial class PackageCreationWizard {
        /// <summary>
        /// Builds the runtime asmdef JSON for the generated package.
        /// </summary>
        public string GetRuntimeAsmDef() {
            return $@"{{
    ""name"": ""{_assemblyName}"",
    ""rootNamespace"": ""{_namespaceName}"",
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
        public string GetSamplesAsmDef() {
            // TODO: ideally we'd need to add a reference to the Runtime asmdef here, but the GUID is not known
            // until the project is first opened to generate the .meta file
            return $@"{{
    ""name"": ""{_assemblyName}.Samples"",
    ""rootNamespace"": ""{_namespaceName}.Samples"",
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
        /// Builds the editor-only asmdef JSON for the generated package.
        /// </summary>
        public string GetEditorAsmDef() {
            return $@"{{
    ""name"": ""{_assemblyName}.Editor"",
    ""rootNamespace"": ""{_namespaceName}.Editor"",
    ""references"": [
        ""{_assemblyName}""
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
