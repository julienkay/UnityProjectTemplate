namespace Doji.PackageAuthoring.Editor.Wizards.Templates {
    /// <summary>
    /// Builds asmdef files for generated package assemblies.
    /// </summary>
    public static class AsmDefTemplate {
        /// <summary>
        /// Builds the runtime asmdef JSON for the generated package.
        /// </summary>
        public static string GetRuntimeAsmDef(PackageContext ctx) {
            return $@"{{
    ""name"": ""{ctx.Package.AssemblyName}"",
    ""rootNamespace"": ""{ctx.Package.NamespaceName}"",
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
        public static string GetSamplesAsmDef(PackageContext ctx, string runtimeAssemblyGuid) {
            return $@"{{
    ""name"": ""{ctx.Package.AssemblyName}.Samples"",
    ""rootNamespace"": ""{ctx.Package.NamespaceName}.Samples"",
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
        public static string GetEditorAsmDef(PackageContext ctx, string runtimeAssemblyGuid) {
            return $@"{{
    ""name"": ""{ctx.Package.AssemblyName}.Editor"",
    ""rootNamespace"": ""{ctx.Package.NamespaceName}.Editor"",
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
