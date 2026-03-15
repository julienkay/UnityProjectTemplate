using System;
using UnityEditor;
using UnityEngine;

namespace Doji.PackageAuthoring.Editor.Wizards {
    /// <summary>
    /// Shared IMGUI section chrome used by the creation wizards, settings page, and preset inspector.
    /// </summary>
    internal static class CreationWizardLayout {
        /// <summary>
        /// Draws a boxed section with a bold header and body content.
        /// </summary>
        /// <param name="title">Section title displayed in the header row.</param>
        /// <param name="drawContent">Section body renderer.</param>
        public static void DrawSection(string title, Action drawContent) {
            DrawSection(title, drawContent, null);
        }

        /// <summary>
        /// Draws a boxed section with an optional right-aligned header action such as a preset icon.
        /// </summary>
        /// <param name="title">Section title displayed in the header row.</param>
        /// <param name="drawContent">Section body renderer.</param>
        /// <param name="drawHeaderAction">Optional right-aligned header content renderer.</param>
        public static void DrawSection(string title, Action drawContent, Action drawHeaderAction) {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            drawHeaderAction?.Invoke();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(3f);
            drawContent?.Invoke();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Preset icon used inside section header rows.
        /// </summary>
        /// <param name="iconTooltip">Tooltip shown when hovering the preset button.</param>
        /// <param name="onPresetClicked">Callback invoked with the button rect when the icon is pressed.</param>
        public static void DrawSectionHeaderPresetButton(string iconTooltip, Action<Rect> onPresetClicked) {
            if (GUILayout.Button(
                    EditorGUIUtility.IconContent("d_Preset.Context", iconTooltip),
                    EditorStyles.iconButton,
                    GUILayout.Width(24f),
                    GUILayout.Height(20f))) {
                onPresetClicked?.Invoke(GUILayoutUtility.GetLastRect());
            }
        }
    }
}
