using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;

namespace Doji.PackageAuthoring.Editor.Wizards.Presets {
    /// <summary>
    /// Builds the shared preset context menu used by the wizards and the settings provider.
    /// </summary>
    internal static class PackageAuthoringPresetMenu {
        /// <summary>
        /// Shows a context menu that can apply either project defaults or one of the preset assets in the project.
        /// </summary>
        /// <param name="buttonRect">Unused anchor information kept for compatibility with current callers.</param>
        /// <param name="applyProjectDefaults">Invoked when the caller wants to restore project defaults.</param>
        /// <param name="applyPreset">Invoked when a preset asset is chosen from the menu.</param>
        /// <param name="includeProjectDefaults">Whether the menu should include the project-defaults entry.</param>
        public static void Show(
            Rect buttonRect,
            Action applyProjectDefaults,
            Action<PackageAuthoringDefaults> applyPreset,
            bool includeProjectDefaults = true) {
            var menu = new GenericMenu();
            if (includeProjectDefaults) {
                menu.AddItem(new GUIContent("Project Defaults"), false, () => applyProjectDefaults?.Invoke());
                menu.AddSeparator(string.Empty);
            }

            List<PackageAuthoringDefaults> presets = FindPresets();
            if (presets.Count == 0) {
                menu.AddDisabledItem(new GUIContent("Presets/No Presets Found"));
            }
            else {
                foreach (PackageAuthoringDefaults preset in presets) {
                    PackageAuthoringDefaults capturedPreset = preset;
                    menu.AddItem(
                        new GUIContent($"Presets/{capturedPreset.name}"),
                        false,
                        () => applyPreset?.Invoke(capturedPreset));
                }
            }

            menu.ShowAsContext();
        }

        /// <summary>
        /// All preset assets available in the current Unity project.
        /// </summary>
        private static List<PackageAuthoringDefaults> FindPresets() {
            return AssetDatabase.FindAssets("t:PackageAuthoringDefaults")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => !string.IsNullOrWhiteSpace(path))
                .Select(AssetDatabase.LoadAssetAtPath<PackageAuthoringDefaults>)
                .Where(preset => preset != null)
                .OrderBy(preset => preset.name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
