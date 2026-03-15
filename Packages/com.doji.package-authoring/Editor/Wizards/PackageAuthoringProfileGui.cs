using System;
using UnityEditor;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;
using Doji.PackageAuthoring.Editor.Wizards.PackageSearch;
using UnityEngine;

namespace Doji.PackageAuthoring.Editor.Wizards {
    /// <summary>
    /// Shared top-level IMGUI rendering for serialized <see cref="PackageAuthoringProfile"/> instances.
    /// </summary>
    internal static class PackageAuthoringProfileGui {
        private static readonly string ProjectDefaultsField = $"<{nameof(PackageAuthoringProfile.ProjectDefaults)}>k__BackingField";
        private static readonly string PackageDefaultsField = $"<{nameof(PackageAuthoringProfile.PackageDefaults)}>k__BackingField";
        private static readonly string RepoDefaultsField = $"<{nameof(PackageAuthoringProfile.RepoDefaults)}>k__BackingField";
        private static readonly string TargetLocationField = $"<{nameof(ProjectSettings.TargetLocation)}>k__BackingField";

        /// <summary>
        /// Finds the serialized project-defaults block on the provided profile object.
        /// </summary>
        public static SerializedProperty FindProjectDefaultsProperty(SerializedObject profileObject) {
            return profileObject.FindProperty(ProjectDefaultsField);
        }

        /// <summary>
        /// Finds the serialized package-defaults block on the provided profile object.
        /// </summary>
        public static SerializedProperty FindPackageDefaultsProperty(SerializedObject profileObject) {
            return profileObject.FindProperty(PackageDefaultsField);
        }

        /// <summary>
        /// Finds the serialized repository-defaults block on the provided profile object.
        /// </summary>
        public static SerializedProperty FindRepoDefaultsProperty(SerializedObject profileObject) {
            return profileObject.FindProperty(RepoDefaultsField);
        }

        /// <summary>
        /// Draws the package-defaults section for the provided serialized profile.
        /// </summary>
        public static void DrawPackageSettingsSection(
            SerializedObject profileObject,
            string title,
            UnityRegistryPackageAutocompleteField.SuggestionOverflowMode overflowMode =
                UnityRegistryPackageAutocompleteField.SuggestionOverflowMode.Scroll,
            Action drawHeaderAction = null,
            Action drawFooter = null) {
            using var _ = PackageSettingsDrawerContext.Push(overflowMode);
            CreationWizardLayout.DrawSection(title, () => {
                EditorGUILayout.PropertyField(
                    FindPackageDefaultsProperty(profileObject),
                    GUIContent.none,
                    includeChildren: true);
                drawFooter?.Invoke();
            }, drawHeaderAction);
        }

        /// <summary>
        /// Draws the repository-defaults section for the provided serialized profile.
        /// </summary>
        public static void DrawRepoSettingsSection(
            SerializedObject profileObject,
            string title,
            Action drawHeaderAction = null,
            Action drawFooter = null) {
            CreationWizardLayout.DrawSection(title, () => {
                EditorGUILayout.PropertyField(
                    FindRepoDefaultsProperty(profileObject),
                    GUIContent.none,
                    includeChildren: true);
                drawFooter?.Invoke();
            }, drawHeaderAction);
        }

        /// <summary>
        /// Draws the project-defaults section for the provided serialized profile.
        /// </summary>
        public static void DrawProjectSettingsSection(
            SerializedObject profileObject,
            string title,
            string productLabel = "Product Name",
            bool includeTargetLocation = true,
            Action drawHeaderAction = null,
            Action drawFooter = null) {
            using var _ = ProjectSettingsDrawerContext.Push(productLabel, includeTargetLocation);
            CreationWizardLayout.DrawSection(title, () => {
                EditorGUILayout.PropertyField(
                    FindProjectDefaultsProperty(profileObject),
                    GUIContent.none,
                    includeChildren: true);
                drawFooter?.Invoke();
            }, drawHeaderAction);
        }

        /// <summary>
        /// Draws the editable output-location field from the serialized project-defaults block.
        /// </summary>
        public static void DrawProjectOutputField(SerializedObject profileObject) {
            EditorGUILayout.PropertyField(
                FindProjectDefaultsProperty(profileObject).FindPropertyRelative(TargetLocationField),
                new GUIContent("Target Location"));
        }
    }
}
