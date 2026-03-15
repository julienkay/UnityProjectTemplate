using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Wizards.Models;
using Doji.PackageAuthoring.Editor.Wizards.Presets;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace Doji.PackageAuthoring.Editor.Wizards {
    /// <summary>
    /// Editor window that scaffolds a standalone Unity project from the shared project defaults or a preset.
    /// </summary>
    public class ProjectCreationWizard : EditorWindow {
        private const string ProjectSectionPresetTooltip = "Apply project defaults or a preset asset.";

        [SerializeField] private ProjectSettings _projectSettings = new();
        [SerializeField] private bool _initializedFromDefaults;
        private bool _autoOpenAfterCreation = true;

        private string ProjectDirectory => Path.Combine(_projectSettings.TargetLocation, _projectSettings.ProductName);

        private static string _originalCompanyName;
        private static string _originalProductName;
        private static string _originalVersion;
        private static readonly List<string> OriginalIdentifiers = new();

        private static readonly List<NamedBuildTarget> NamedTargets = new() {
            NamedBuildTarget.Standalone,
            NamedBuildTarget.Android
        };

        private static string _originalRootNamespace;

        [MenuItem("Tools/Project Creation Wizard")]
        public static void ShowWindow() {
            GetWindow<ProjectCreationWizard>().titleContent = new GUIContent("Project Creation");
        }

        /// <summary>
        /// Initializes the window title and seeds the initial ad hoc state from project defaults.
        /// </summary>
        private void OnEnable() {
            titleContent = new GUIContent("Project Creation");

            if (_initializedFromDefaults) {
                return;
            }

            ApplyProjectDefaults();
            _initializedFromDefaults = true;
        }

        /// <summary>
        /// Draws the standalone project wizard UI.
        /// </summary>
        private void OnGUI() {
            GUILayout.Space(10);
            CreationWizardLayout.DrawSection(
                "Project Settings",
                DrawProjectSettingsSection,
                () => CreationWizardLayout.DrawSectionHeaderPresetButton(ProjectSectionPresetTooltip, ShowPresetMenu));

            GUILayout.Space(8f);
            CreationWizardLayout.DrawSection("Output", DrawOutputSection);

            GUILayout.Space(10f);
            if (GUILayout.Button("Create Project")) {
                CreateProjectStructure();
            }
        }

        /// <summary>
        /// Resets the current in-window state from the project-wide defaults.
        /// </summary>
        private void ApplyProjectDefaults() {
            _projectSettings.CopyFrom(PackageAuthoringProjectSettings.instance.ProjectDefaults);
        }

        /// <summary>
        /// Applies only the project-facing portion of a preset to the current ad hoc window state.
        /// </summary>
        private void ApplyPreset(PackageAuthoringDefaults preset) {
            if (preset == null) {
                ApplyProjectDefaults();
                return;
            }

            _projectSettings.CopyFrom(preset.ProjectDefaults);
        }

        /// <summary>
        /// Restores project defaults and repaints the window so the UI reflects the new state immediately.
        /// </summary>
        private void ApplyProjectDefaultsAndRefresh() {
            ApplyProjectDefaults();
            GUI.FocusControl(null);
            Repaint();
        }

        /// <summary>
        /// Applies the selected preset and repaints the window.
        /// </summary>
        private void ApplyPresetAndRefresh(PackageAuthoringDefaults preset) {
            ApplyPreset(preset);
            GUI.FocusControl(null);
            Repaint();
        }

        /// <summary>
        /// Opens the shared preset context menu for the standalone project wizard.
        /// </summary>
        private void ShowPresetMenu(Rect buttonRect) {
            PackageAuthoringPresetMenu.Show(
                buttonRect,
                ApplyProjectDefaultsAndRefresh,
                ApplyPresetAndRefresh);
        }

        /// <summary>
        /// Draws editable project identity fields specific to standalone project creation.
        /// </summary>
        private void DrawProjectSettingsSection() {
            CreationWizardLayout.DrawProjectIdentityFields(_projectSettings, productLabel: "Project Name");
            _autoOpenAfterCreation = EditorGUILayout.Toggle("Auto-Open After Creation", _autoOpenAfterCreation);
        }

        /// <summary>
        /// Draws the destination fields and resolved output folder preview.
        /// </summary>
        private void DrawOutputSection() {
            _projectSettings.TargetLocation =
                EditorGUILayout.TextField("Target Location", _projectSettings.TargetLocation);
            EditorGUILayout.LabelField("Project Folder", ProjectDirectory, EditorStyles.miniLabel);
        }

        /// <summary>
        /// Copies the template project into the chosen location and optionally opens it in Unity.
        /// </summary>
        private void CreateProjectStructure() {
            Directory.CreateDirectory(ProjectDirectory);
            UpdateProjectSettings();

            try {
                Directory.CreateDirectory(Path.Combine(ProjectDirectory, "Assets"));
                CopyDirectory("Packages", Path.Combine(ProjectDirectory, "Packages"));
                CopyDirectory("ProjectSettings", Path.Combine(ProjectDirectory, "ProjectSettings"));
            }
            finally {
                RevertProjectSettings();
            }

            string gitignorePath = Path.Combine(Directory.GetCurrentDirectory(), ".gitignore");
            if (File.Exists(gitignorePath)) {
                string targetPath = Path.Combine(ProjectDirectory, ".gitignore");
                CopyFile(gitignorePath, targetPath);
            }

            Debug.Log($"Project created successfully at {ProjectDirectory}");

            if (_autoOpenAfterCreation) {
                OpenProjectInUnity(ProjectDirectory);
            }
        }

        /// <summary>
        /// Temporarily applies the wizard values to Unity project settings before copying the template project.
        /// </summary>
        private void UpdateProjectSettings() {
            _originalCompanyName = PlayerSettings.companyName;
            _originalProductName = PlayerSettings.productName;
            _originalVersion = PlayerSettings.bundleVersion;
            OriginalIdentifiers.Clear();
            foreach (var target in NamedTargets) {
                OriginalIdentifiers.Add(PlayerSettings.GetApplicationIdentifier(target));
            }

            _originalRootNamespace = EditorSettings.projectGenerationRootNamespace;

            PlayerSettings.companyName = _projectSettings.CompanyName;
            PlayerSettings.productName = _projectSettings.ProductName;
            PlayerSettings.bundleVersion = _projectSettings.Version;
            foreach (var target in NamedTargets) {
                PlayerSettings.SetApplicationIdentifier(target,
                    $"com.{_projectSettings.CompanyName.ToLower().Replace(" ", "")}.{_projectSettings.ProductName.ToLower()}");
            }

            EditorSettings.projectGenerationRootNamespace = _projectSettings.ProductName;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Restores the original Unity project settings after scaffolding completes.
        /// </summary>
        private static void RevertProjectSettings() {
            PlayerSettings.companyName = _originalCompanyName;
            PlayerSettings.productName = _originalProductName;
            PlayerSettings.bundleVersion = _originalVersion;
            for (int i = 0; i < OriginalIdentifiers.Count; i++) {
                PlayerSettings.SetApplicationIdentifier(NamedTargets[i], OriginalIdentifiers[i]);
            }

            EditorSettings.projectGenerationRootNamespace = _originalRootNamespace;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Recursively copies a directory tree into the generated standalone project.
        /// </summary>
        private void CopyDirectory(string sourceDir, string destinationDir) {
            if (!Directory.Exists(sourceDir)) {
                Debug.LogWarning($"Source directory {sourceDir} does not exist. Skipping copy.");
                return;
            }

            Directory.CreateDirectory(destinationDir);

            foreach (string file in Directory.GetFiles(sourceDir)) {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                CopyFile(file, destFile);
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir)) {
                string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                CopyDirectory(subDir, destSubDir);
            }
        }

        /// <summary>
        /// Copies a single file if the destination does not already exist.
        /// </summary>
        private void CopyFile(string sourceFileName, string destFileName) {
            if (!File.Exists(destFileName)) {
                File.Copy(sourceFileName, destFileName, overwrite: false);
            }
        }

        /// <summary>
        /// Opens the generated standalone project in the currently running Unity editor.
        /// </summary>
        private void OpenProjectInUnity(string projectPath) {
            string editorPath = EditorApplication.applicationPath; // path to current Unity Editor
            if (!File.Exists(editorPath)) {
                Debug.LogError("Could not locate Unity Editor executable to open new project.");
                return;
            }

            Process.Start(new ProcessStartInfo {
                FileName = editorPath,
                Arguments = $"-projectPath \"{projectPath}\"",
                UseShellExecute = false
            });

            Debug.Log($"Opening project in Unity: {projectPath}");
        }
    }
}
