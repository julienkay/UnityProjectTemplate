using UnityEditor;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;
using Doji.PackageAuthoring.Editor.Wizards;

namespace Doji.PackageAuthoring.Editor.Wizards.Presets {
    /// <summary>
    /// Registers and renders the Project Settings page for package authoring defaults.
    /// </summary>
    internal static class PackageAuthoringSettingsProvider {
        private static readonly string DependenciesField = $"<{nameof(PackageScaffoldSettings.Dependencies)}>k__BackingField";

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
                    EditorGUIUtility.IconContent("d_Preset.Context", "Apply a preset asset to the current project defaults."),
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
            var settings = PackageAuthoringProjectSettings.instance;
            using var serializedSettings = new SerializedObject(settings);
            SerializedProperty packageDefaultsProperty = serializedSettings.FindProperty("packageDefaults");
            SerializedProperty packageDependenciesProperty = packageDefaultsProperty.FindPropertyRelative(DependenciesField);
            serializedSettings.Update();

            EditorGUILayout.Space(8f);
            EditorGUI.BeginChangeCheck();
            CreationWizardLayout.DrawSection("Package Defaults", () => {
                CreationWizardLayout.DrawPackageSettingsFields(settings.PackageDefaults);
                EditorGUILayout.Space(6f);
                CreationWizardLayout.DrawPackageContentFields(settings.PackageDefaults);
                EditorGUILayout.Space(6f);
                EditorGUILayout.PropertyField(packageDependenciesProperty, includeChildren: true);
            });

            EditorGUILayout.Space(8f);
            CreationWizardLayout.DrawSection("Repo Defaults", () => {
                CreationWizardLayout.DrawRepoSettingsFields(settings.RepoDefaults);
            });

            EditorGUILayout.Space(8f);
            CreationWizardLayout.DrawSection("Project Defaults", () => {
                CreationWizardLayout.DrawProjectIdentityFields(settings.ProjectDefaults, productLabel: "Project Name");
                settings.ProjectDefaults.TargetLocation =
                    EditorGUILayout.TextField("Target Location", settings.ProjectDefaults.TargetLocation);
            });

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
        private static void ApplyPresetToProjectDefaults(PackageAuthoringDefaults preset) {
            if (preset == null) {
                return;
            }

            var settings = PackageAuthoringProjectSettings.instance;
            settings.ProjectDefaults.CopyFrom(preset.ProjectDefaults);
            settings.PackageDefaults.CopyFrom(preset.PackageDefaults);
            settings.RepoDefaults.CopyFrom(preset.RepoDefaults);
            settings.SaveSettings();
        }
    }
}
