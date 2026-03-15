using UnityEditor;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;

namespace Doji.PackageAuthoring.Editor.Wizards {
    /// <summary>
    /// Draws the serialized project-settings block used by the project and package authoring tools.
    /// </summary>
    [CustomPropertyDrawer(typeof(ProjectSettings))]
    internal sealed class ProjectSettingsDrawer : PropertyDrawer {
        private static readonly string CompanyNameField = $"<{nameof(ProjectSettings.CompanyName)}>k__BackingField";
        private static readonly string ProductNameField = $"<{nameof(ProjectSettings.ProductName)}>k__BackingField";
        private static readonly string VersionField = $"<{nameof(ProjectSettings.Version)}>k__BackingField";

        private static readonly string TargetLocationField =
            $"<{nameof(ProjectSettings.TargetLocation)}>k__BackingField";

        /// <inheritdoc />
        public override bool CanCacheInspectorGUI(SerializedProperty property) {
            return false;
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            ProjectSettingsDrawerContext.State state = ProjectSettingsDrawerContext.Current;
            int lineCount = state.IncludeTargetLocation ? 4 : 3;
            return (EditorGUIUtility.singleLineHeight * lineCount) +
                   (EditorGUIUtility.standardVerticalSpacing * (lineCount - 1));
        }

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            ProjectSettingsDrawerContext.State state = ProjectSettingsDrawerContext.Current;
            Rect row = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginProperty(position, label, property);
            DrawField(ref row, property.FindPropertyRelative(CompanyNameField), new GUIContent("Company Name"));
            DrawField(ref row, property.FindPropertyRelative(ProductNameField), new GUIContent(state.ProductLabel));
            DrawField(ref row, property.FindPropertyRelative(VersionField), new GUIContent("Version"));

            if (state.IncludeTargetLocation) {
                DrawField(ref row, property.FindPropertyRelative(TargetLocationField),
                    new GUIContent("Target Location"));
            }

            EditorGUI.EndProperty();
        }

        private static void DrawField(ref Rect row, SerializedProperty property, GUIContent label) {
            EditorGUI.PropertyField(row, property, label);
            row.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
