using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Utilities;
using Doji.PackageAuthoring.Editor.Wizards.Models;
using Doji.PackageAuthoring.Editor.Wizards.Presets;

namespace Doji.PackageAuthoring.Editor.Wizards {
    /// <summary>
    /// Editor window that scaffolds a standalone Unity project from the shared project defaults or a preset.
    /// </summary>
    public class ProjectCreationWizard : EditorWindow {
        private const string ProjectSectionPresetTooltip = "Apply project defaults or a preset asset.";
        private static readonly string ProductNameField =
            $"<{nameof(Doji.PackageAuthoring.Editor.Wizards.Models.ProjectSettings.ProductName)}>k__BackingField";
        private static readonly string TargetLocationField =
            $"<{nameof(Doji.PackageAuthoring.Editor.Wizards.Models.ProjectSettings.TargetLocation)}>k__BackingField";

        [SerializeField] private bool _initializedFromDefaults;
        [SerializeField] private bool _autoOpenAfterCreation = true;

        private string ProjectDirectory => Path.Combine(ProjectSettings.TargetLocation, ProjectSettings.ProductName);

        private static string _originalCompanyName;
        private static string _originalProductName;
        private static string _originalVersion;
        private static readonly List<string> OriginalIdentifiers = new();

        private static readonly List<NamedBuildTarget> NamedTargets = new() {
            NamedBuildTarget.Standalone,
            NamedBuildTarget.Android
        };

        private static string _originalRootNamespace;
        private PackageAuthoringProfile _defaults;
        private SerializedObject _defaultsSerializedObject;
        private SerializedObject _windowSerializedObject;
        private SerializedProperty _autoOpenAfterCreationProperty;

        private PackageAuthoringProfile Defaults => _defaults ??= CreateTemporaryProfile();
        private ProjectSettings ProjectSettings => Defaults.ProjectDefaults;

        [MenuItem("Tools/Project Creation Wizard")]
        public static void ShowWindow() {
            GetWindow<ProjectCreationWizard>().titleContent = new GUIContent("Project Creation");
        }

        /// <summary>
        /// Initializes the window title and seeds the initial ad hoc state from project defaults.
        /// </summary>
        private void OnEnable() {
            titleContent = new GUIContent("Project Creation");

            if (!_initializedFromDefaults) {
                ApplyProjectDefaults();
                _initializedFromDefaults = true;
            }

            InitializeSerializedState();
        }

        private void OnDisable() {
            if (_defaults != null) {
                DestroyImmediate(_defaults);
                _defaults = null;
            }

            _defaultsSerializedObject = null;
            _windowSerializedObject = null;
            _autoOpenAfterCreationProperty = null;
        }

        /// <summary>
        /// Draws the standalone project wizard UI.
        /// </summary>
        private void OnGUI() {
            if (_defaultsSerializedObject == null || _windowSerializedObject == null) {
                InitializeSerializedState();
            }

            _defaultsSerializedObject.Update();
            _windowSerializedObject.Update();
            GUILayout.Space(10);
            DrawProjectSettingsSection();

            GUILayout.Space(8f);
            CreationWizardLayout.DrawSection("Output", DrawOutputSection);

            _defaultsSerializedObject.ApplyModifiedProperties();
            _windowSerializedObject.ApplyModifiedProperties();

            GUILayout.Space(10f);
            if (GUILayout.Button("Create Project")) {
                CreateProjectStructure();
            }
        }

        /// <summary>
        /// Resets the current in-window state from the project-wide defaults.
        /// </summary>
        private void ApplyProjectDefaults() {
            Defaults.CopyFrom(PackageAuthoringProjectSettings.Instance);
        }

        /// <summary>
        /// Applies only the project-facing portion of a preset to the current ad hoc window state.
        /// </summary>
        private void ApplyPreset(PackageAuthoringProfile preset) {
            if (preset == null) {
                ApplyProjectDefaults();
                return;
            }

            Defaults.CopyFrom(preset);
        }

        /// <summary>
        /// Restores project defaults and repaints the window so the UI reflects the new state immediately.
        /// </summary>
        private void ApplyProjectDefaultsAndRefresh() {
            ApplyProjectDefaults();
            GUI.FocusControl(null);
            _defaultsSerializedObject?.Update();
            _windowSerializedObject?.Update();
            Repaint();
        }

        /// <summary>
        /// Applies the selected preset and repaints the window.
        /// </summary>
        private void ApplyPresetAndRefresh(PackageAuthoringProfile preset) {
            ApplyPreset(preset);
            GUI.FocusControl(null);
            _defaultsSerializedObject?.Update();
            _windowSerializedObject?.Update();
            Repaint();
        }

        /// <summary>
        /// Rebuilds cached serialized properties after domain reloads or window recreation.
        /// </summary>
        private void InitializeSerializedState() {
            _defaultsSerializedObject = new SerializedObject(Defaults);
            _windowSerializedObject = new SerializedObject(this);
            _autoOpenAfterCreationProperty = _windowSerializedObject.FindProperty(nameof(_autoOpenAfterCreation));
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
            PackageAuthoringProfileGui.DrawProjectSettingsSection(
                _defaultsSerializedObject,
                "Project Settings",
                productLabel: "Project Name",
                includeTargetLocation: false,
                drawHeaderAction: () => CreationWizardLayout.DrawSectionHeaderPresetButton(
                    ProjectSectionPresetTooltip,
                    ShowPresetMenu),
                drawFooter: () => EditorGUILayout.PropertyField(
                    _autoOpenAfterCreationProperty,
                    new GUIContent("Auto-Open After Creation")));
        }

        /// <summary>
        /// Draws the destination fields and resolved output folder preview.
        /// </summary>
        private void DrawOutputSection() {
            PackageAuthoringProfileGui.DrawProjectOutputField(_defaultsSerializedObject);
            EditorGUILayout.LabelField("Project Folder", PreviewProjectDirectory, EditorStyles.miniLabel);
        }

        private string PreviewProjectDirectory => Path.Combine(CurrentTargetLocation, CurrentProductName);
        private string CurrentProductName => GetSerializedString(
            PackageAuthoringProfileGui.FindProjectDefaultsProperty(_defaultsSerializedObject),
            ProductNameField,
            ProjectSettings.ProductName);
        private string CurrentTargetLocation => GetSerializedString(
            PackageAuthoringProfileGui.FindProjectDefaultsProperty(_defaultsSerializedObject),
            TargetLocationField,
            ProjectSettings.TargetLocation);

        private static string GetSerializedString(SerializedProperty property, string relativePath, string fallback) {
            return property?.FindPropertyRelative(relativePath)?.stringValue ?? fallback;
        }

        private static PackageAuthoringProfile CreateTemporaryProfile() {
            var profile = CreateInstance<PackageAuthoringProfile>();
            profile.hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor;
            profile.ProjectDefaults = new ProjectSettings {
                ProductName = "MyPackage"
            };
            profile.PackageDefaults = new PackageSettings();
            profile.RepoDefaults = new RepoSettings();
            return profile;
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
                UnityEditorLauncherUtility.TryOpenProjectInCurrentEditor(ProjectDirectory);
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

            PlayerSettings.companyName = ProjectSettings.CompanyName;
            PlayerSettings.productName = ProjectSettings.ProductName;
            PlayerSettings.bundleVersion = ProjectSettings.Version;
            foreach (var target in NamedTargets) {
                PlayerSettings.SetApplicationIdentifier(target,
                    $"com.{ProjectSettings.CompanyName.ToLower().Replace(" ", "")}.{ProjectSettings.ProductName.ToLower()}");
            }

            EditorSettings.projectGenerationRootNamespace = ProjectSettings.ProductName;

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

    }
}
