using UnityEditor;
using Doji.PackageAuthoring.Editor.Wizards.Models;
using Doji.PackageAuthoring.Editor.Wizards.UI;

namespace Doji.PackageAuthoring.Editor.Wizards.Presets {
    /// <summary>
    /// Renders package authoring preset assets with the shared grouped UI.
    /// </summary>
    [CustomEditor(typeof(PackageAuthoringProfile), true)]
    internal sealed class PackageAuthoringProfileEditor : UnityEditor.Editor {
        /// <summary>
        /// Draws the preset asset inspector using the shared authoring sections.
        /// </summary>
        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.Space(4f);
            PackageAuthoringGui.DrawPackageSettingsSection(serializedObject, "Package Defaults");

            EditorGUILayout.Space(8f);
            PackageAuthoringGui.DrawRepoSettingsSection(serializedObject, "Repo Defaults");

            EditorGUILayout.Space(8f);
            PackageAuthoringGui.DrawProjectSettingsSection(
                serializedObject,
                "Project Defaults",
                productLabel: "Project Name",
                includeTargetLocation: true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
