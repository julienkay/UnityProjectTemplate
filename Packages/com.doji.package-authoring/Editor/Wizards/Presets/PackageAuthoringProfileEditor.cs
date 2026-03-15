using UnityEditor;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;

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
            PackageAuthoringProfileGui.DrawPackageSettingsSection(serializedObject, "Package Defaults");

            EditorGUILayout.Space(8f);
            PackageAuthoringProfileGui.DrawRepoSettingsSection(serializedObject, "Repo Defaults");

            EditorGUILayout.Space(8f);
            PackageAuthoringProfileGui.DrawProjectSettingsSection(
                serializedObject,
                "Project Defaults",
                productLabel: "Project Name",
                includeTargetLocation: true);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
