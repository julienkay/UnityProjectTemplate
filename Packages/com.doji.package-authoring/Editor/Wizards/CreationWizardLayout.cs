using System;
using UnityEditor;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;

namespace Doji.PackageAuthoring.Editor.Wizards {
    /// <summary>
    /// Shared IMGUI layout helpers for the creation wizards and settings page.
    /// </summary>
    internal static class CreationWizardLayout {
        private static readonly GUIContent LicenseTypeLabel = EditorGUIUtility.TrTextContent(
            "License Type",
            "Controls the generated license template.");

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

        /// <summary>
        /// Common project identity fields shared by the project and package wizards.
        /// </summary>
        public static void DrawProjectIdentityFields(
            ProjectScaffoldSettings projectSettings,
            string productLabel = "Product Name") {
            projectSettings.CompanyName = EditorGUILayout.TextField("Company Name", projectSettings.CompanyName);
            projectSettings.ProductName = EditorGUILayout.TextField(productLabel, projectSettings.ProductName);
            projectSettings.Version = EditorGUILayout.TextField("Version", projectSettings.Version);
        }

        /// <summary>
        /// Core package metadata fields shown in presets, project settings, and the package wizard.
        /// </summary>
        public static void DrawPackageSettingsFields(PackageScaffoldSettings packageSettings) {
            packageSettings.PackageName = EditorGUILayout.TextField("Identifier", packageSettings.PackageName);
            packageSettings.AssemblyName = EditorGUILayout.TextField("Assembly Name", packageSettings.AssemblyName);
            packageSettings.NamespaceName = EditorGUILayout.TextField("Namespace", packageSettings.NamespaceName);
            packageSettings.Description = EditorGUILayout.TextField("Description", packageSettings.Description);
            packageSettings.CompanyName = EditorGUILayout.TextField("Company Name", packageSettings.CompanyName);
            packageSettings.IncludeAuthor = EditorGUILayout.Toggle("Include Author Metadata", packageSettings.IncludeAuthor);
            if (packageSettings.IncludeAuthor) {
                EditorGUI.indentLevel++;
                packageSettings.AuthorUrl = EditorGUILayout.TextField("URL", packageSettings.AuthorUrl);
                packageSettings.AuthorEmail = EditorGUILayout.TextField("Email", packageSettings.AuthorEmail);
                EditorGUI.indentLevel--;
            }
            packageSettings.IncludeMinimumUnityVersion =
                EditorGUILayout.Toggle("Minimum Unity Version", packageSettings.IncludeMinimumUnityVersion);
            if (packageSettings.IncludeMinimumUnityVersion) {
                EditorGUI.indentLevel++;
                packageSettings.MinimumUnityMajor = EditorGUILayout.TextField("Major", packageSettings.MinimumUnityMajor);
                packageSettings.MinimumUnityMinor = EditorGUILayout.TextField("Minor", packageSettings.MinimumUnityMinor);
                packageSettings.MinimumUnityRelease =
                    EditorGUILayout.TextField("Release", packageSettings.MinimumUnityRelease);
                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// Package-content toggles that control optional folder scaffolding.
        /// </summary>
        public static void DrawPackageContentFields(PackageScaffoldSettings packageSettings) {
            packageSettings.CreateDocsFolder =
                EditorGUILayout.Toggle("Create Documentation Folder", packageSettings.CreateDocsFolder);
            packageSettings.CreateSamplesFolder =
                EditorGUILayout.Toggle("Create Samples Folder", packageSettings.CreateSamplesFolder);
            packageSettings.CreateEditorFolder =
                EditorGUILayout.Toggle("Create Editor Folder", packageSettings.CreateEditorFolder);
            packageSettings.CreateTestsFolder =
                EditorGUILayout.Toggle("Create Tests Folder", packageSettings.CreateTestsFolder);
        }

        /// <summary>
        /// Repository-level fields shared by package scaffolding defaults and the package wizard.
        /// </summary>
        public static void DrawRepoSettingsFields(RepoScaffoldSettings repoSettings) {
            repoSettings.CopyrightHolder =
                EditorGUILayout.TextField("Copyright Holder", repoSettings.CopyrightHolder);
            repoSettings.LicenseType =
                (LicenseType)EditorGUILayout.EnumPopup(LicenseTypeLabel, repoSettings.LicenseType);
        }
    }
}
