namespace Doji.PackageAuthoring.Editor.Wizards.Templates {
    /// <summary>
    /// Provides short template call sites on top of <see cref="PackageContext"/>.
    /// </summary>
    public static class PackageTemplateContextExtensions {
        /// <summary>
        /// Builds the runtime asmdef JSON for the generated package.
        /// </summary>
        public static string GetRuntimeAsmDef(this PackageContext ctx) {
            return AsmDefTemplate.GetRuntimeAsmDef(ctx);
        }

        /// <summary>
        /// Builds the samples asmdef JSON for the generated package.
        /// </summary>
        public static string GetSamplesAsmDef(this PackageContext ctx, string runtimeAssemblyGuid) {
            return AsmDefTemplate.GetSamplesAsmDef(ctx, runtimeAssemblyGuid);
        }

        /// <summary>
        /// Builds the editor-only asmdef JSON for the generated package.
        /// </summary>
        public static string GetEditorAsmDef(this PackageContext ctx, string runtimeAssemblyGuid) {
            return AsmDefTemplate.GetEditorAsmDef(ctx, runtimeAssemblyGuid);
        }

        /// <summary>
        /// Builds the runtime assembly info file.
        /// </summary>
        public static string GetAssemblyInfo(this PackageContext ctx) {
            return AssemblyInfoTemplate.GetAssemblyInfo(ctx);
        }

        /// <summary>
        /// Builds the repository license file.
        /// </summary>
        public static string GetLicense(this PackageContext ctx) {
            return LicenseTemplate.GetLicense(ctx);
        }

        /// <summary>
        /// Builds the package manifest.
        /// </summary>
        public static string GetPackageManifest(this PackageContext ctx) {
            return PackageManifestTemplate.GetPackageManifest(ctx);
        }

        /// <summary>
        /// Builds the companion project manifest.
        /// </summary>
        public static string GetProjectManifest(this PackageContext ctx) {
            return ProjectManifestTemplate.GetProjectManifest(ctx);
        }

        /// <summary>
        /// Builds the package README.
        /// </summary>
        public static string GetPackageReadme(this PackageContext ctx) {
            return ReadmeTemplate.GetPackageReadme(ctx);
        }

        /// <summary>
        /// Builds the repository README.
        /// </summary>
        public static string GetRepositoryReadme(this PackageContext ctx) {
            return ReadmeTemplate.GetRepositoryReadme(ctx);
        }

        /// <summary>
        /// Builds the starter sample script.
        /// </summary>
        public static string GetSampleScript(this PackageContext ctx) {
            return ScriptTemplates.GetSampleScript(ctx);
        }

        /// <summary>
        /// Builds the docfx configuration file.
        /// </summary>
        public static string GetDocfxJson(this PackageContext ctx) {
            return DocfxTemplates.GetDocfxJson(ctx);
        }

        /// <summary>
        /// Builds the docfx PDF configuration file.
        /// </summary>
        public static string GetDocfxPdfJson(this PackageContext ctx) {
            return DocfxTemplates.GetDocfxPdfJson(ctx);
        }

        /// <summary>
        /// Builds the docfx API filter file.
        /// </summary>
        public static string GetFilterConfig(this PackageContext ctx) {
            return DocfxTemplates.GetFilterConfig(ctx);
        }

        /// <summary>
        /// Builds the docs landing page.
        /// </summary>
        public static string GetIndexMD(this PackageContext ctx) {
            return DocfxTemplates.GetIndexMD(ctx);
        }

        /// <summary>
        /// Builds the root documentation table of contents.
        /// </summary>
        public static string GetRootToc(this PackageContext ctx) {
            return DocfxTemplates.GetRootToc(ctx);
        }

        /// <summary>
        /// Builds the manual documentation table of contents.
        /// </summary>
        public static string GetManualToc(this PackageContext ctx) {
            return DocfxTemplates.GetManualToc(ctx);
        }

        /// <summary>
        /// Builds the changelog for the current version.
        /// </summary>
        public static string GetChangelog(this PackageContext ctx) {
            return ChangelogTemplate.GetContent(ctx.Project.Version);
        }
    }
}
