using UnityEditor;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;

namespace Doji.PackageAuthoring.Editor.Wizards {
    /// <summary>
    /// Draws the serialized package-settings block and delegates dependency editing to the nested list drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(PackageSettings))]
    internal sealed class PackageSettingsDrawer : PropertyDrawer {
        private static readonly string PackageNameField = $"<{nameof(PackageSettings.PackageName)}>k__BackingField";
        private static readonly string AssemblyNameField = $"<{nameof(PackageSettings.AssemblyName)}>k__BackingField";
        private static readonly string NamespaceNameField = $"<{nameof(PackageSettings.NamespaceName)}>k__BackingField";
        private static readonly string DescriptionField = $"<{nameof(PackageSettings.Description)}>k__BackingField";
        private static readonly string CompanyNameField = $"<{nameof(PackageSettings.CompanyName)}>k__BackingField";
        private static readonly string IncludeAuthorField = $"<{nameof(PackageSettings.IncludeAuthor)}>k__BackingField";
        private static readonly string AuthorUrlField = $"<{nameof(PackageSettings.AuthorUrl)}>k__BackingField";
        private static readonly string AuthorEmailField = $"<{nameof(PackageSettings.AuthorEmail)}>k__BackingField";
        private static readonly string IncludeMinimumUnityVersionField =
            $"<{nameof(PackageSettings.IncludeMinimumUnityVersion)}>k__BackingField";
        private static readonly string MinimumUnityMajorField = $"<{nameof(PackageSettings.MinimumUnityMajor)}>k__BackingField";
        private static readonly string MinimumUnityMinorField = $"<{nameof(PackageSettings.MinimumUnityMinor)}>k__BackingField";
        private static readonly string MinimumUnityReleaseField =
            $"<{nameof(PackageSettings.MinimumUnityRelease)}>k__BackingField";
        private static readonly string CreateDocsFolderField = $"<{nameof(PackageSettings.CreateDocsFolder)}>k__BackingField";
        private static readonly string CreateSamplesFolderField = $"<{nameof(PackageSettings.CreateSamplesFolder)}>k__BackingField";
        private static readonly string CreateEditorFolderField = $"<{nameof(PackageSettings.CreateEditorFolder)}>k__BackingField";
        private static readonly string CreateTestsFolderField = $"<{nameof(PackageSettings.CreateTestsFolder)}>k__BackingField";
        private static readonly string DependenciesField = $"<{nameof(PackageSettings.Dependencies)}>k__BackingField";

        /// <inheritdoc />
        public override bool CanCacheInspectorGUI(SerializedProperty property) {
            return false;
        }

        /// <inheritdoc />
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var lineCount = 11;
            if (property.FindPropertyRelative(IncludeAuthorField).boolValue) {
                lineCount += 2;
            }

            if (property.FindPropertyRelative(IncludeMinimumUnityVersionField).boolValue) {
                lineCount += 3;
            }

            float lineHeight = (EditorGUIUtility.singleLineHeight * lineCount) +
                               (EditorGUIUtility.standardVerticalSpacing * (lineCount - 1));
            SerializedProperty dependenciesProperty = property.FindPropertyRelative(DependenciesField);
            float dependenciesHeight = EditorGUI.GetPropertyHeight(
                dependenciesProperty,
                GUIContent.none,
                includeChildren: true);
            return lineHeight + 8f + dependenciesHeight;
        }

        /// <inheritdoc />
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            Rect row = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.BeginProperty(position, label, property);
            DrawField(ref row, property.FindPropertyRelative(PackageNameField), new GUIContent("Identifier"));
            DrawField(ref row, property.FindPropertyRelative(AssemblyNameField), new GUIContent("Assembly Name"));
            DrawField(ref row, property.FindPropertyRelative(NamespaceNameField), new GUIContent("Namespace"));
            DrawField(ref row, property.FindPropertyRelative(DescriptionField), new GUIContent("Description"));
            DrawField(ref row, property.FindPropertyRelative(CompanyNameField), new GUIContent("Company Name"));

            SerializedProperty includeAuthorProperty = property.FindPropertyRelative(IncludeAuthorField);
            DrawField(ref row, includeAuthorProperty, new GUIContent("Include Author Metadata"));
            if (includeAuthorProperty.boolValue) {
                EditorGUI.indentLevel++;
                DrawField(ref row, property.FindPropertyRelative(AuthorUrlField), new GUIContent("URL"));
                DrawField(ref row, property.FindPropertyRelative(AuthorEmailField), new GUIContent("Email"));
                EditorGUI.indentLevel--;
            }

            SerializedProperty includeUnityVersionProperty =
                property.FindPropertyRelative(IncludeMinimumUnityVersionField);
            DrawField(ref row, includeUnityVersionProperty, new GUIContent("Minimum Unity Version"));
            if (includeUnityVersionProperty.boolValue) {
                EditorGUI.indentLevel++;
                DrawField(ref row, property.FindPropertyRelative(MinimumUnityMajorField), new GUIContent("Major"));
                DrawField(ref row, property.FindPropertyRelative(MinimumUnityMinorField), new GUIContent("Minor"));
                DrawField(ref row, property.FindPropertyRelative(MinimumUnityReleaseField), new GUIContent("Release"));
                EditorGUI.indentLevel--;
            }

            DrawField(ref row, property.FindPropertyRelative(CreateDocsFolderField), new GUIContent("Create Documentation Folder"));
            DrawField(ref row, property.FindPropertyRelative(CreateSamplesFolderField), new GUIContent("Create Samples Folder"));
            DrawField(ref row, property.FindPropertyRelative(CreateEditorFolderField), new GUIContent("Create Editor Folder"));
            DrawField(ref row, property.FindPropertyRelative(CreateTestsFolderField), new GUIContent("Create Tests Folder"));

            row.y += 8f - EditorGUIUtility.standardVerticalSpacing;
            SerializedProperty dependenciesProperty = property.FindPropertyRelative(DependenciesField);
            float dependenciesHeight = EditorGUI.GetPropertyHeight(
                dependenciesProperty,
                GUIContent.none,
                includeChildren: true);
            EditorGUI.PropertyField(
                new Rect(row.x, row.y, row.width, dependenciesHeight),
                dependenciesProperty,
                new GUIContent("Dependencies"),
                includeChildren: true);
            EditorGUI.EndProperty();
        }

        private static void DrawField(ref Rect row, SerializedProperty property, GUIContent label) {
            EditorGUI.PropertyField(row, property, label);
            row.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}
