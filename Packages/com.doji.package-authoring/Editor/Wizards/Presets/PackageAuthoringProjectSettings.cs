using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;

namespace Doji.PackageAuthoring.Editor.Wizards.Presets {
    /// <summary>
    /// Project-scoped persistent defaults used when the package authoring tools are opened without applying a preset.
    /// This keeps a cached in-memory instance so editor UI state keyed by target instance remains stable across repaints.
    /// </summary>
    [FilePath("ProjectSettings/PackageAuthoringProjectSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class PackageAuthoringProjectSettings : PackageAuthoringProfile {
        private static PackageAuthoringProjectSettings _instance;

        /// <summary>
        /// Shared project settings asset used as the default authoring profile.
        /// Unlike <c>ScriptableSingleton</c>, loading and saving is owned explicitly so this type can inherit the shared profile base.
        /// </summary>
        public static PackageAuthoringProjectSettings Instance => _instance ??= GetOrCreateSettings();

        /// <summary>
        /// Saves the current settings instance back into the project settings asset.
        /// </summary>
        public void SaveSettings() {
            Save(true);
        }

        private void OnDisable() {
            if (_instance == this) {
                _instance = null;
            }
        }

        // Returning the same object instance across GUI passes is important because some editor controls
        // cache transient UI state by target object identity rather than purely by property path.
        private static PackageAuthoringProjectSettings GetOrCreateSettings() {
            PackageAuthoringProjectSettings settings = LoadOrCreate();
            if (settings.ProjectDefaults == null) {
                settings.ProjectDefaults = new ProjectSettings {
                    ProductName = "MyPackage"
                };
            }

            settings.PackageDefaults ??= new PackageSettings();
            settings.RepoDefaults ??= new RepoSettings();
            return settings;
        }

        private static PackageAuthoringProjectSettings LoadOrCreate() {
            Object[] settings =
                InternalEditorUtility.LoadSerializedFileAndForget(
                    "ProjectSettings/PackageAuthoringProjectSettings.asset");
            if (settings.Length > 0 && settings[0] is PackageAuthoringProjectSettings projectSettings) {
                return projectSettings;
            }

            PackageAuthoringProjectSettings created = CreateInstance<PackageAuthoringProjectSettings>();
            created.hideFlags = HideFlags.HideAndDontSave;
            return created;
        }

        private void Save(bool saveAsText) {
            if (ProjectDefaults == null) {
                ProjectDefaults = new ProjectSettings {
                    ProductName = "MyPackage"
                };
            }

            PackageDefaults ??= new PackageSettings();
            RepoDefaults ??= new RepoSettings();
            InternalEditorUtility.SaveToSerializedFileAndForget(
                new Object[] {
                    this
                },
                "ProjectSettings/PackageAuthoringProjectSettings.asset",
                saveAsText);
        }
    }
}
