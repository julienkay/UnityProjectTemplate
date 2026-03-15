using Doji.PackageAuthoring.Editor.Wizards.Models;

namespace Doji.PackageAuthoring.Editor.Wizards.Templates {
    /// <summary>
    /// Carries the current scaffold configuration into template generators without coupling them to the wizard UI.
    /// </summary>
    public sealed class PackageContext {
        /// <summary>
        /// Current companion project configuration.
        /// </summary>
        public ProjectSettings Project { get; }

        /// <summary>
        /// Current package configuration.
        /// </summary>
        public PackageSettings Package { get; }

        /// <summary>
        /// Current repository-level configuration.
        /// </summary>
        public RepoSettings Repo { get; }

        public PackageContext(
            ProjectSettings project,
            PackageSettings package,
            RepoSettings repo) {
            Project = project;
            Package = package;
            Repo = repo;
        }
    }
}
