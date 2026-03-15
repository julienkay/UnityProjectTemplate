namespace Doji.PackageAuthoring.Editor.Wizards {
    /// <summary>
    /// Builds Unity meta file content for generated assets so their GUIDs are stable before first import.
    /// </summary>
    public static class AssetMetaTemplate {
        /// <summary>
        /// Builds meta file content for an assembly definition asset.
        /// </summary>
        public static string GetAsmDefMeta(string guid) {
            return GetContent(guid, "AssemblyDefinitionImporter");
        }

        public static string GetContent(string guid, string importerName) {
            return $@"fileFormatVersion: 2
guid: {guid}
{importerName}:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
";
        }
    }
}
