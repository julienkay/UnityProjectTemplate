using UnityEditor;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards;
using Doji.PackageAuthoring.Editor.Wizards.Models;
using Doji.PackageAuthoring.Editor.Wizards.PackageSearch;

namespace Doji.PackageAuthoring.Editor.Wizards.Presets {
    /// <summary>
    /// Registers and renders the Project Settings page for package authoring defaults.
    /// </summary>
    internal static class PackageAuthoringSettingsProvider {
        /// <summary>
        /// Creates the settings provider shown under <c>Project/Doji/Package Authoring</c>.
        /// </summary>
        [SettingsProvider]
        public static SettingsProvider CreateProvider() {
            var provider = new SettingsProvider("Project/Doji/Package Authoring", SettingsScope.Project) {
                guiHandler = _ => DrawSettingsGui(),
                titleBarGuiHandler = DrawTitleBarGui
            };

            return provider;
        }

        /// <summary>
        /// Draws the preset button that lives in the built-in Settings window title bar.
        /// </summary>
        private static void DrawTitleBarGui() {
            if (!GUILayout.Button(
                    EditorGUIUtility.IconContent("d_Preset.Context",
                        "Apply a preset asset to the current project defaults."),
                    EditorStyles.iconButton,
                    GUILayout.Width(24f),
                    GUILayout.Height(20f))) {
                return;
            }

            PackageAuthoringPresetMenu.Show(
                buttonRect: default,
                applyProjectDefaults: null,
                applyPreset: ApplyPresetToProjectDefaults,
                includeProjectDefaults: false);
        }

        /// <summary>
        /// Draws the editable project-wide defaults for package and project scaffolding.
        /// </summary>
        private static void DrawSettingsGui() {
            var settings = PackageAuthoringProjectSettings.Instance;
            using var serializedSettings = new SerializedObject(settings);
            serializedSettings.Update();

            EditorGUILayout.Space(8f);
            EditorGUI.BeginChangeCheck();
            PackageAuthoringGui.DrawPackageSettingsSection(
                serializedSettings,
                "Package Defaults",
                overflowMode: UnityRegistryPackageAutocompleteField.SuggestionOverflowMode.Scroll);

            EditorGUILayout.Space(8f);
            PackageAuthoringGui.DrawRepoSettingsSection(serializedSettings, "Repo Defaults");

            EditorGUILayout.Space(8f);
            PackageAuthoringGui.DrawProjectSettingsSection(
                serializedSettings,
                "Project Defaults",
                productLabel: "Project Name",
                includeTargetLocation: true);

            if (EditorGUI.EndChangeCheck()) {
                serializedSettings.ApplyModifiedProperties();
                settings.SaveSettings();
                return;
            }

            serializedSettings.ApplyModifiedProperties();
        }

        /// <summary>
        /// Copies the selected preset asset into the current project-wide defaults.
        /// </summary>
        /// <param name="preset">The preset asset selected from the title-bar menu.</param>
        private static void ApplyPresetToProjectDefaults(PackageAuthoringProfile preset) {
            if (preset == null) {
                return;
            }

            var settings = PackageAuthoringProjectSettings.Instance;
            settings.CopyFrom(preset);
            settings.SaveSettings();
        }
    }
}
