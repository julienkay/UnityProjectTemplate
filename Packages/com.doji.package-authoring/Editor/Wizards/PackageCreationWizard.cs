using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditorInternal;
using UnityEngine;
using Doji.PackageAuthoring.Editor.Utilities;
using Doji.PackageAuthoring.Editor.Wizards.Models;
using Doji.PackageAuthoring.Editor.Wizards.PackageSearch;
using Doji.PackageAuthoring.Editor.Wizards.Presets;

namespace Doji.PackageAuthoring.Editor.Wizards {
    /// <summary>
    /// Editor window that scaffolds a package repository and a companion Unity test project.
    /// </summary>
    public partial class PackageCreationWizard : EditorWindow {
        private const string PackageSectionPresetTooltip = "Apply package defaults or a package preset asset.";
        private const string CompanionProjectPresetTooltip = "Apply project defaults or a preset asset to the companion project.";
        private static readonly string DependenciesField = $"<{nameof(PackageScaffoldSettings.Dependencies)}>k__BackingField";
        private static readonly string PackageDependencyPackageNameField =
            $"<{nameof(PackageDependencyEntry.PackageName)}>k__BackingField";
        private static readonly string PackageDependencyVersionField =
            $"<{nameof(PackageDependencyEntry.Version)}>k__BackingField";

        [SerializeField] private ProjectScaffoldSettings _projectSettings = new() { ProductName = "MyPackage" };
        [SerializeField] private PackageScaffoldSettings _packageSettings = new();
        [SerializeField] private RepoScaffoldSettings _repoSettings = new();
        [SerializeField] private bool _initializedFromDefaults;

        private string RootDirectory => Path.Combine(_projectSettings.TargetLocation, _packageSettings.PackageName);
        private string PackageDirectory => Path.Combine(RootDirectory, _packageSettings.PackageName);
        private string ProjectDirectory => Path.Combine(RootDirectory, "projects", _projectSettings.ProductName);

        [SerializeField] private ReorderableList _dependenciesList;
        private UnityRegistryPackageAutocompleteField _dependencyAutocompleteField;

        /// <summary>
        /// Caches reusable GUI content for the dependency list header.
        /// </summary>
        private static class Styles {
            public static readonly GUIContent Version =
                EditorGUIUtility.TrTextContent("Version", "Must follow SemVer (ex: 1.0.0-preview.1).");

            public static readonly GUIContent Package =
                EditorGUIUtility.TrTextContent("Package name", "Must be lowercase");
        }

        private SerializedObject _serializedObject;

        /// <summary>
        /// Opens the package creation wizard.
        /// </summary>
        [MenuItem("Tools/Package Creation Wizard")]
        public static void ShowWindow() {
            GetWindow<PackageCreationWizard>().titleContent = new GUIContent("Package Creation");
        }

        /// <summary>
        /// Initializes the window title, transient state, and dependency editor UI.
        /// </summary>
        private void OnEnable() {
            titleContent = new GUIContent("Package Creation");

            if (!_initializedFromDefaults) {
                ApplyProjectDefaults();
                _initializedFromDefaults = true;
            }

            _serializedObject = new SerializedObject(this);
            CreateDependencyAutocompleteField();
            PackageSearchCache.Shared.EnsureLoaded();

            _dependenciesList = new ReorderableList(
                _serializedObject,
                _serializedObject.FindProperty(nameof(_packageSettings)).FindPropertyRelative(DependenciesField),
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true) {
                drawElementCallback = DrawDependencyListElement,
                drawHeaderCallback = DrawDependencyHeaderElement,
                elementHeightCallback = GetDependencyElementHeight
            };
        }

        /// <summary>
        /// Resets both the package and companion-project portions of the current window state from project defaults.
        /// </summary>
        private void ApplyProjectDefaults() {
            var projectSettings = PackageAuthoringProjectSettings.instance;
            _projectSettings.CopyFrom(projectSettings.ProjectDefaults);
            _packageSettings.CopyFrom(projectSettings.PackageDefaults);
            _repoSettings.CopyFrom(projectSettings.RepoDefaults);
        }

        /// <summary>
        /// Applies only the shared project-facing portion of a preset to the companion-project section.
        /// </summary>
        private void ApplyProjectPreset(PackageAuthoringDefaults preset) {
            if (preset == null) {
                ApplyProjectDefaultsToCompanionProject();
                return;
            }

            _projectSettings.CopyFrom(preset.ProjectDefaults);
        }

        /// <summary>
        /// Applies only the package-facing portion of a preset to the package-definition section.
        /// </summary>
        private void ApplyPackagePreset(PackageAuthoringDefaults preset) {
            if (preset == null) {
                ApplyProjectDefaultsToPackageDefinition();
                return;
            }

            _packageSettings.CopyFrom(preset.PackageDefaults);
            _repoSettings.CopyFrom(preset.RepoDefaults);
        }

        /// <summary>
        /// Releases transient editor-only helpers when the window closes or recompiles.
        /// </summary>
        private void OnDisable() {
            _dependencyAutocompleteField?.Dispose();
            _dependencyAutocompleteField = null;
        }

        /// <summary>
        /// Recreates the dependency autocomplete field that is embedded in each reorderable-list row.
        /// </summary>
        private void CreateDependencyAutocompleteField() {
            _dependencyAutocompleteField?.Dispose();
            _dependencyAutocompleteField = new UnityRegistryPackageAutocompleteField(
                Repaint,
                overflowMode: UnityRegistryPackageAutocompleteField.SuggestionOverflowMode.Scroll);
        }

        /// <summary>
        /// Draws the dependency list header labels.
        /// </summary>
        private void DrawDependencyHeaderElement(Rect rect) {
            var w = rect.width;
            rect.x += 4;
            rect.width = w / 3 * 2 - 2;
            GUI.Label(rect, Styles.Package, EditorStyles.label);

            rect.x += w / 3 * 2;
            rect.width = w / 3 - 4;
            GUI.Label(rect, Styles.Version, EditorStyles.label);
        }

        /// <summary>
        /// Draws one package dependency row using the shared autocomplete field.
        /// </summary>
        private void DrawDependencyListElement(Rect rect, int index, bool isActive, bool isFocused) {
            var list = _dependenciesList.serializedProperty;
            var dependency = list.GetArrayElementAtIndex(index);
            var packageNameProperty = dependency.FindPropertyRelative(PackageDependencyPackageNameField);
            var versionProperty = dependency.FindPropertyRelative(PackageDependencyVersionField);

            _dependencyAutocompleteField.Draw(rect, GetDependencyFieldKey(index), packageNameProperty, versionProperty);
        }

        /// <summary>
        /// Returns the dynamic row height required for a dependency entry and its autocomplete suggestions.
        /// </summary>
        private float GetDependencyElementHeight(int index) {
            var list = _dependenciesList.serializedProperty;
            var dependency = list.GetArrayElementAtIndex(index);
            var packageNameProperty = dependency.FindPropertyRelative(PackageDependencyPackageNameField);
            return _dependencyAutocompleteField.GetHeight(GetDependencyFieldKey(index), packageNameProperty.stringValue);
        }

        /// <summary>
        /// Builds a stable per-row control key for the dependency autocomplete UI.
        /// </summary>
        private string GetDependencyFieldKey(int index) {
            return $"{GetInstanceID()}.dependency.{index}";
        }

        /// <summary>
        /// Draws the package creation wizard UI.
        /// </summary>
        private void OnGUI() {
            _serializedObject.Update();
            GUILayout.Space(10);
            CreationWizardLayout.DrawSection(
                "Package Definition",
                DrawPackageDefinitionSection,
                () => CreationWizardLayout.DrawSectionHeaderPresetButton(PackageSectionPresetTooltip, ShowPackagePresetMenu));

            GUILayout.Space(8f);
            CreationWizardLayout.DrawSection("Repo Settings", DrawRepoSettingsSection);

            GUILayout.Space(8f);
            CreationWizardLayout.DrawSection(
                "Companion Project",
                DrawCompanionProjectSection,
                () => CreationWizardLayout.DrawSectionHeaderPresetButton(CompanionProjectPresetTooltip, ShowCompanionProjectPresetMenu));

            GUILayout.Space(8f);
            CreationWizardLayout.DrawSection("Output", DrawOutputSection);

            _serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Create Package")) {
                CreatePackageScaffolding();
            }
        }

        /// <summary>
        /// Restores only the package-definition portion of the window state from project defaults.
        /// </summary>
        private void ApplyProjectDefaultsToPackageDefinition() {
            _packageSettings.CopyFrom(PackageAuthoringProjectSettings.instance.PackageDefaults);
            _repoSettings.CopyFrom(PackageAuthoringProjectSettings.instance.RepoDefaults);
        }

        /// <summary>
        /// Restores only the companion-project portion of the window state from project defaults.
        /// </summary>
        private void ApplyProjectDefaultsToCompanionProject() {
            _projectSettings.CopyFrom(PackageAuthoringProjectSettings.instance.ProjectDefaults);
        }

        /// <summary>
        /// Restores package defaults and refreshes the window immediately.
        /// </summary>
        private void ApplyPackageDefaultsAndRefresh() {
            ApplyProjectDefaultsToPackageDefinition();
            GUI.FocusControl(null);
            _serializedObject?.Update();
            Repaint();
        }

        /// <summary>
        /// Restores companion-project defaults and refreshes the window immediately.
        /// </summary>
        private void ApplyCompanionProjectDefaultsAndRefresh() {
            ApplyProjectDefaultsToCompanionProject();
            GUI.FocusControl(null);
            _serializedObject?.Update();
            Repaint();
        }

        /// <summary>
        /// Applies the package portion of a selected preset and refreshes the window.
        /// </summary>
        private void ApplyPackagePresetAndRefresh(PackageAuthoringDefaults preset) {
            ApplyPackagePreset(preset);
            GUI.FocusControl(null);
            _serializedObject?.Update();
            Repaint();
        }

        /// <summary>
        /// Applies the project portion of a selected preset and refreshes the window.
        /// </summary>
        private void ApplyProjectPresetAndRefresh(PackageAuthoringDefaults preset) {
            ApplyProjectPreset(preset);
            GUI.FocusControl(null);
            _serializedObject?.Update();
            Repaint();
        }

        /// <summary>
        /// Opens the preset menu scoped to package-definition defaults.
        /// </summary>
        private void ShowPackagePresetMenu(Rect buttonRect) {
            PackageAuthoringPresetMenu.Show(
                buttonRect,
                ApplyPackageDefaultsAndRefresh,
                ApplyPackagePresetAndRefresh);
        }

        /// <summary>
        /// Opens the preset menu scoped to companion-project defaults.
        /// </summary>
        private void ShowCompanionProjectPresetMenu(Rect buttonRect) {
            PackageAuthoringPresetMenu.Show(
                buttonRect,
                ApplyCompanionProjectDefaultsAndRefresh,
                ApplyProjectPresetAndRefresh);
        }

        /// <summary>
        /// Draws the package-definition section, including optional content toggles and dependencies.
        /// </summary>
        private void DrawPackageDefinitionSection() {
            CreationWizardLayout.DrawPackageSettingsFields(_packageSettings);
            CreationWizardLayout.DrawPackageContentFields(_packageSettings);

            GUILayout.Space(8f);
            EditorGUILayout.LabelField("Dependencies", EditorStyles.boldLabel);
            _dependenciesList.DoLayoutList();
        }

        /// <summary>
        /// Draws the repository-level section that controls generated root metadata such as the license file.
        /// </summary>
        private void DrawRepoSettingsSection() {
            CreationWizardLayout.DrawRepoSettingsFields(_repoSettings);
        }

        /// <summary>
        /// Draws the companion-project section that controls the generated sample or test project metadata.
        /// </summary>
        private void DrawCompanionProjectSection() {
            CreationWizardLayout.DrawProjectIdentityFields(_projectSettings, productLabel: "Project Name");
            EditorGUILayout.HelpBox(
                "These values are applied to the generated companion Unity project and shared where the package uses the same product metadata.",
                MessageType.None);
        }

        /// <summary>
        /// Draws the output paths resolved from the current package and project settings.
        /// </summary>
        private void DrawOutputSection() {
            _projectSettings.TargetLocation = EditorGUILayout.TextField("Target Location", _projectSettings.TargetLocation);
            EditorGUILayout.LabelField("Repository Root", RootDirectory, EditorStyles.miniLabel);
            EditorGUILayout.LabelField("Package Folder", PackageDirectory, EditorStyles.miniLabel);
            EditorGUILayout.LabelField("Companion Project", ProjectDirectory, EditorStyles.miniLabel);
        }

        /// <summary>
        /// Creates the package repository skeleton, optional folders, and the companion Unity project.
        /// </summary>
        private void CreatePackageScaffolding() {
            Directory.CreateDirectory(RootDirectory);
            Directory.CreateDirectory(PackageDirectory);
            Directory.CreateDirectory(ProjectDirectory);

            if (_packageSettings.CreateDocsFolder) {
                CreateDocsFolder(RootDirectory);
            }

            CreatePackageFolders();
            CreateProjectStructure();
            CreateRootFiles();

            GitUtility.InitializeRepository(RootDirectory, _packageSettings.PackageName);

            Debug.Log($"Package scaffolding created successfully at {RootDirectory}");
        }

        /// <summary>
        /// Creates the package folders that live inside the generated package root.
        /// </summary>
        private void CreatePackageFolders() {
            CreateRuntimeFolder();

            if (_packageSettings.CreateSamplesFolder) {
                CreateSamplesFolder();
            }

            if (_packageSettings.CreateEditorFolder) {
                CreateEditorFolder();
            }

            if (_packageSettings.CreateTestsFolder) {
                Directory.CreateDirectory(Path.Combine(PackageDirectory, "Tests"));
            }

            // Package metadata is written after the folder layout is in place so optional folders can influence it.
            CreatePackageFiles(PackageDirectory);
        }

        /// <summary>
        /// Copies the template Unity project and points its manifest back to the generated local package.
        /// </summary>
        private void CreateProjectStructure() {
            UpdateProjectSettings();

            try {
                Directory.CreateDirectory(Path.Combine(ProjectDirectory, "Assets"));

                CopyDirectory("Packages", Path.Combine(ProjectDirectory, "Packages"));
                CopyDirectory("ProjectSettings", Path.Combine(ProjectDirectory, "ProjectSettings"));

                string gitignorePath = Path.Combine(Directory.GetCurrentDirectory(), ".gitignore");
                if (File.Exists(gitignorePath)) {
                    string targetPath = Path.Combine(ProjectDirectory, ".gitignore");
                    CopyFile(gitignorePath, targetPath);
                }

                string projectManifestPath = Path.Combine(ProjectDirectory, "Packages", "manifest.json");
                CreateFile(projectManifestPath, GetProjectManifest(), overwrite: true);
            }
            finally {
                // The generator temporarily mutates project-level settings so copied files contain the new package metadata.
                RevertProjectSettings();
            }
        }

        private static string _originalCompanyName;
        private static string _originalProductName;
        private static string _originalVersion;
        private static readonly List<string> OriginalIdentifiers = new();

        private static readonly List<NamedBuildTarget> NamedTargets = new() {
            NamedBuildTarget.Standalone,
            NamedBuildTarget.Android
        };

        private static string _originalRootNamespace;

        /// <summary>
        /// Temporarily applies the wizard values to <see cref="PlayerSettings"/> so copied project files inherit them.
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
                PlayerSettings.SetApplicationIdentifier(target, _packageSettings.PackageName);
            }

            EditorSettings.projectGenerationRootNamespace = _packageSettings.NamespaceName;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Restores the user's original project settings after the template project has been copied.
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

        private void CreateRootFiles() {
            string licensePath = Path.Combine(RootDirectory, "LICENSE");
            CreateFile(licensePath, GetLicense());

            string readmePath = Path.Combine(RootDirectory, "README.md");
            CreateFile(readmePath, GetRepositoryReadme());
        }

        private void CreatePackageFiles(string path) {
            string packageManifestPath = Path.Combine(path, "package.json");
            // `package.json` is regenerated because optional samples and dependencies directly affect its contents.
            CreateFile(packageManifestPath, GetPackageManifest(), overwrite: true);

            string readmePath = Path.Combine(path, "README.md");
            CreateFile(readmePath, GetPackageReadme());

            string changelogPath = Path.Combine(path, "CHANGELOG.md");
            CreateFile(changelogPath, ChangelogTemplate.GetContent(_projectSettings.Version));
        }

        /// <summary>
        /// Recursively copies a directory into the generated output, preserving any existing destination files.
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
        /// Creates the runtime assembly definition in the provided folder.
        /// </summary>
        private void CreateRuntimeAsmDef(string path) {
            string asmDefPath = Path.Combine(path, $"{_packageSettings.AssemblyName}.asmdef");
            CreateFile(asmDefPath, GetRuntimeAsmDef());
        }

        /// <summary>
        /// Creates the samples assembly definition in the provided folder.
        /// </summary>
        private void CreateSamplesAsmDef(string path) {
            string asmDefPath = Path.Combine(path, $"{_packageSettings.AssemblyName}.asmdef");
            CreateFile(asmDefPath, GetSamplesAsmDef());
        }

        /// <summary>
        /// Creates the editor-only assembly definition in the provided folder.
        /// </summary>
        private void CreateEditorAsmDef(string path) {
            string asmDefPath = Path.Combine(path, $"{_packageSettings.AssemblyName}.Editor.asmdef");
            CreateFile(asmDefPath, GetEditorAsmDef());
        }

        /// <summary>
        /// Creates the runtime assembly info file in the provided folder.
        /// </summary>
        private void CreateAssemblyInfo(string path) {
            string assemblyInfoPath = Path.Combine(path, "AssemblyInfo.cs");
            CreateFile(assemblyInfoPath, GetAssemblyInfo());
        }

        /// <summary>
        /// Creates the runtime folder and its baseline assembly files.
        /// </summary>
        private void CreateRuntimeFolder() {
            string runtimePath = Path.Combine(PackageDirectory, "Runtime");
            Directory.CreateDirectory(runtimePath);
            CreateRuntimeAsmDef(runtimePath);
            CreateAssemblyInfo(runtimePath);
        }

        /// <summary>
        /// Creates the editor folder and its editor-only assembly definition.
        /// </summary>
        private void CreateEditorFolder() {
            string editorPath = Path.Combine(PackageDirectory, "Editor");
            Directory.CreateDirectory(editorPath);
            CreateEditorAsmDef(editorPath);
        }

        /// <summary>
        /// Creates the samples root, assembly definition, and starter sample script.
        /// </summary>
        private void CreateSamplesFolder() {
            string samplesPath = Path.Combine(PackageDirectory, "Samples~");
            Directory.CreateDirectory(samplesPath);

            Directory.CreateDirectory(Path.Combine(samplesPath, "00-SharedSampleAssets"));
            Directory.CreateDirectory(Path.Combine(samplesPath, "01-BasicSample"));
            CreateSamplesAsmDef(samplesPath);

            // Keep the starter sample in a numbered folder so package manager ordering is predictable.
            CreateFile(Path.Combine(samplesPath, "01-BasicSample", "BasicSample.cs"), GetSampleScript());
        }

        /// <summary>
        /// Creates the documentation folder and the generated docfx configuration files.
        /// </summary>
        private void CreateDocsFolder(string path) {
            path = Path.Combine(path, "docs");
            Directory.CreateDirectory(path);
            CreateDocfxFolders(path);
            CreateDocfxFiles(path);
        }

        /// <summary>
        /// Copies any repository-level docs template content into the generated docs folder.
        /// </summary>
        private void CreateDocfxFolders(string path) {
            CopyDirectory("docs", path);
        }

        /// <summary>
        /// Writes the generated docfx configuration and table-of-contents files.
        /// </summary>
        private void CreateDocfxFiles(string path) {
            string docfxConfigPath = Path.Combine(path, "docfx.json");
            CreateFile(docfxConfigPath, GetDocfxJson());

            string docfxPdfConfigPath = Path.Combine(path, "docfx-pdf.json");
            CreateFile(docfxPdfConfigPath, GetDocfxPdfJson());

            string filterConfigPath = Path.Combine(path, "filterConfig.yml");
            CreateFile(filterConfigPath, GetFilterConfig());

            string indexPath = Path.Combine(path, "index.md");
            CreateFile(indexPath, GetIndexMD());

            string tocPath = Path.Combine(path, "toc.yml");
            CreateFile(tocPath, GetRootToc());

            string manualTocPath = Path.Combine(path, "manual", "toc.yml");
            CreateFile(manualTocPath, GetManualToc());
        }

        private void CreateFile(string path, string content, bool overwrite = false) {
            if (!File.Exists(path) || overwrite) {
                File.WriteAllText(path, content);
            }
        }

        private void CopyFile(string sourceFileName, string destFileName) {
            if (!File.Exists(destFileName)) {
                File.Copy(sourceFileName, destFileName, overwrite: false);
            }
        }
    }
}
