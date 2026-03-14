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
        public string GetSamplesAsmDef() {
            // TODO: ideally we'd need to add a reference to the Runtime asmdef here, but the GUID is not known
            // until the project is first opened to generate the .meta file
            return $@"{{
    ""name"": ""{_packageSettings.AssemblyName}.Samples"",
    ""rootNamespace"": ""{_packageSettings.NamespaceName}.Samples"",
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
    ""name"": ""{_packageSettings.AssemblyName}.Editor"",
    ""rootNamespace"": ""{_packageSettings.NamespaceName}.Editor"",
    ""references"": [
        ""{_packageSettings.AssemblyName}""
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
