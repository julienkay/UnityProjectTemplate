using UnityEditor;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;

namespace Doji.PackageAuthoring.Editor.Wizards {
    /// <summary>
    /// Draws the serialized repository-settings block shared by package authoring surfaces.
    /// </summary>
    [CustomPropertyDrawer(typeof(RepoSettings))]
    internal sealed class RepoSettingsDrawer : PropertyDrawer {
        private static readonly string CopyrightHolderField =
            $"<{nameof(RepoSettings.CopyrightHolder)}>k__BackingField";

        private static readonly string LicenseTypeField = $"<{nameof(RepoSettings.LicenseType)}>k__BackingField";

        private static readonly GUIContent LicenseTypeLabel = EditorGUIUtility.TrTextContent(
            "License Type",
            "Controls the generated license template.");

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return (EditorGUIUtility.singleLineHeight * 2f) + EditorGUIUtility.standardVerticalSpacing;
        }

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Rect row = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(
                row,
                property.FindPropertyRelative(CopyrightHolderField),
                new GUIContent("Copyright Holder"));
            row.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(
                row,
                property.FindPropertyRelative(LicenseTypeField),
                LicenseTypeLabel);
            EditorGUI.EndProperty();
        }
    }
}
