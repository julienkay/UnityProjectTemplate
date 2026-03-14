using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditorInternal;
using UnityEngine;

namespace Doji.PackageAuthoring.Editor {
    /// <summary>
    /// Editor window that scaffolds a package repository and a companion Unity test project.
    /// </summary>
    public partial class PackageCreationWizard : EditorWindow {
        private const string PreferencesKey = "Doji.PackageAuthoring.PackageCreationWizard";
        private const string PackageDependencyPackageNameField = "packageName";
        private const string PackageDependencyVersionField = "version";

        private string _companyName = "Doji Technologies";
        private string _productName = "MyPackage";
        private string _packageName = "com.doji.package";
        private string _assemblyName = "Doji.Package";
        private string _namespaceName = "Doji.AI.PackageNS";
        private string _description = "A short description for readmes etc";
        private string _author = "Julien Kipp";
        private string _version = "1.0.0";
        private LicenseType _selectedLicenseType = LicenseType.MIT;

        private string _targetLocation = "../";
        private string RootDirectory => Path.Combine(_targetLocation, _packageName);
        private string PackageDirectory => Path.Combine(RootDirectory, _packageName);
        private string ProjectDirectory => Path.Combine(RootDirectory, "projects", _productName);

        private bool createDocsFolder = true;
        private bool createSamplesFolder = false;
        private bool createEditorFolder = false;
        private bool createTestsFolder = false;

        [SerializeField]
        private List<PackageDependency> _dependencies = new() {
            new() {
                PackageName = "com.unity.ai.inference",
                Version = "2.3.0"
            }
        };

        [SerializeField] private ReorderableList _dependenciesList;
        private UnityRegistryPackageAutocompleteField _dependencyAutocompleteField;

        private static class Styles {
            public static readonly GUIContent Version =
                EditorGUIUtility.TrTextContent("Version", "Must follow SemVer (ex: 1.0.0-preview.1).");

            public static readonly GUIContent Package =
                EditorGUIUtility.TrTextContent("Package name", "Must be lowercase");
        }

        private SerializedObject _serializedObject;

        /// <summary>
        /// Serializable dependency entry written into the generated package manifest.
        /// </summary>
        [Serializable]
        public class PackageDependency {
            [SerializeField] private string packageName;
            [SerializeField] private string version;

            public string PackageName {
                get => packageName;
                set => packageName = value;
            }

            public string Version {
                get => version;
                set => version = value;
            }
        }

        [Serializable]
        private class WizardPreferences {
            [SerializeField] private string companyName;
            [SerializeField] private string productName;
            [SerializeField] private string packageName;
            [SerializeField] private string assemblyName;
            [SerializeField] private string namespaceName;
            [SerializeField] private string description;
            [SerializeField] private string author;
            [SerializeField] private string version;
            [SerializeField] private int selectedLicenseType;
            [SerializeField] private string targetLocation;
            [SerializeField] private bool createDocsFolder;
            [SerializeField] private bool createSamplesFolder;
            [SerializeField] private bool createEditorFolder;
            [SerializeField] private bool createTestsFolder;
            [SerializeField] private List<PackageDependency> dependencies;

            public string CompanyName {
                get => companyName;
                set => companyName = value;
            }

            public string ProductName {
                get => productName;
                set => productName = value;
            }

            public string PackageName {
                get => packageName;
                set => packageName = value;
            }

            public string AssemblyName {
                get => assemblyName;
                set => assemblyName = value;
            }

            public string NamespaceName {
                get => namespaceName;
                set => namespaceName = value;
            }

            public string Description {
                get => description;
                set => description = value;
            }

            public string Author {
                get => author;
                set => author = value;
            }

            public string Version {
                get => version;
                set => version = value;
            }

            public int SelectedLicenseType {
                get => selectedLicenseType;
                set => selectedLicenseType = value;
            }

            public string TargetLocation {
                get => targetLocation;
                set => targetLocation = value;
            }

            public bool CreateDocsFolder {
                get => createDocsFolder;
                set => createDocsFolder = value;
            }

            public bool CreateSamplesFolder {
                get => createSamplesFolder;
                set => createSamplesFolder = value;
            }

            public bool CreateEditorFolder {
                get => createEditorFolder;
                set => createEditorFolder = value;
            }

            public bool CreateTestsFolder {
                get => createTestsFolder;
                set => createTestsFolder = value;
            }

            public List<PackageDependency> Dependencies {
                get => dependencies;
                set => dependencies = value;
            }
        }

        /// <summary>
        /// Opens the package creation wizard.
        /// </summary>
        [MenuItem("Tools/Package Creation Wizard")]
        public static void ShowWindow() {
            GetWindow<PackageCreationWizard>("Package Creation Wizard");
        }

        private void OnEnable() {
            LoadPreferences();
            _serializedObject = new SerializedObject(this);
            CreateDependencyAutocompleteField();
            PackageSearchCache.Shared.EnsureLoaded();

            _dependenciesList = new ReorderableList(
                _serializedObject,
                _serializedObject.FindProperty(nameof(_dependencies)),
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true) {
                drawElementCallback = DrawDependencyListElement,
                drawHeaderCallback = DrawDependencyHeaderElement,
                elementHeightCallback = GetDependencyElementHeight
            };
        }

        private void OnDisable() {
            _dependencyAutocompleteField?.Dispose();
            _dependencyAutocompleteField = null;
        }

        private void CreateDependencyAutocompleteField() {
            _dependencyAutocompleteField?.Dispose();
            _dependencyAutocompleteField = new UnityRegistryPackageAutocompleteField(
                Repaint,
                overflowMode: UnityRegistryPackageAutocompleteField.SuggestionOverflowMode.Scroll);
        }

        private void DrawDependencyHeaderElement(Rect rect) {
            var w = rect.width;
            rect.x += 4;
            rect.width = w / 3 * 2 - 2;
            GUI.Label(rect, Styles.Package, EditorStyles.label);

            rect.x += w / 3 * 2;
            rect.width = w / 3 - 4;
            GUI.Label(rect, Styles.Version, EditorStyles.label);
        }

        private void DrawDependencyListElement(Rect rect, int index, bool isActive, bool isFocused) {
            var list = _dependenciesList.serializedProperty;
            var dependency = list.GetArrayElementAtIndex(index);
            var packageNameProperty = dependency.FindPropertyRelative(PackageDependencyPackageNameField);
            var versionProperty = dependency.FindPropertyRelative(PackageDependencyVersionField);

            _dependencyAutocompleteField.Draw(rect, GetDependencyFieldKey(index), packageNameProperty, versionProperty);
        }

        private float GetDependencyElementHeight(int index) {
            var list = _dependenciesList.serializedProperty;
            var dependency = list.GetArrayElementAtIndex(index);
            var packageNameProperty = dependency.FindPropertyRelative(PackageDependencyPackageNameField);
            return _dependencyAutocompleteField.GetHeight(GetDependencyFieldKey(index), packageNameProperty.stringValue);
        }

        private string GetDependencyFieldKey(int index) {
            return $"{GetInstanceID()}.dependency.{index}";
        }

        private void OnGUI() {
            _serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            GUILayout.Space(10);
            GUILayout.Label("Package Information", EditorStyles.boldLabel);

            _companyName = EditorGUILayout.TextField("Company Name", _companyName);
            _productName = EditorGUILayout.TextField("Product Name", _productName);
            _packageName = EditorGUILayout.TextField("Identifier", _packageName);
            _assemblyName = EditorGUILayout.TextField("Assembly Name", _assemblyName);
            _namespaceName = EditorGUILayout.TextField("Namespace", _namespaceName);
            _description = EditorGUILayout.TextField("Description", _description);
            _author = EditorGUILayout.TextField("Author", _author);
            _version = EditorGUILayout.TextField("Version", _version);

            _selectedLicenseType = (LicenseType)EditorGUILayout.EnumPopup("License Type", _selectedLicenseType);

            createDocsFolder = EditorGUILayout.Toggle("Create Documentation Folder", createDocsFolder);
            createSamplesFolder = EditorGUILayout.Toggle("Create Samples Folder", createSamplesFolder);
            createEditorFolder = EditorGUILayout.Toggle("Create Editor Folder", createEditorFolder);
            createTestsFolder = EditorGUILayout.Toggle("Create Tests Folder", createTestsFolder);

            GUILayout.Space(10);
            GUILayout.Label("Dependencies (Optional)", EditorStyles.boldLabel);
            _dependenciesList.DoLayoutList();

            _serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck()) {
                SavePreferences();
            }

            if (GUILayout.Button("Create Package")) {
                CreatePackageScaffolding();
            }
        }

        /// <summary>
        /// Loads persisted wizard values for the current Unity project while leaving field defaults intact when none exist.
        /// </summary>
        private void LoadPreferences() {
            string json = EditorUserSettings.GetConfigValue(PreferencesKey);
            if (string.IsNullOrEmpty(json)) {
                return;
            }

            var preferences = JsonUtility.FromJson<WizardPreferences>(json);
            if (preferences == null) {
                return;
            }

            _companyName = preferences.CompanyName ?? _companyName;
            _productName = preferences.ProductName ?? _productName;
            _packageName = preferences.PackageName ?? _packageName;
            _assemblyName = preferences.AssemblyName ?? _assemblyName;
            _namespaceName = preferences.NamespaceName ?? _namespaceName;
            _description = preferences.Description ?? _description;
            _author = preferences.Author ?? _author;
            _version = preferences.Version ?? _version;
            _selectedLicenseType = Enum.IsDefined(typeof(LicenseType), preferences.SelectedLicenseType)
                ? (LicenseType)preferences.SelectedLicenseType
                : _selectedLicenseType;
            _targetLocation = preferences.TargetLocation ?? _targetLocation;
            createDocsFolder = preferences.CreateDocsFolder;
            createSamplesFolder = preferences.CreateSamplesFolder;
            createEditorFolder = preferences.CreateEditorFolder;
            createTestsFolder = preferences.CreateTestsFolder;
            if (preferences.Dependencies != null) {
                _dependencies = CloneDependencies(preferences.Dependencies);
            }
        }

        /// <summary>
        /// Persists the current wizard values into project-scoped editor settings so they survive editor restarts.
        /// </summary>
        private void SavePreferences() {
            var preferences = new WizardPreferences {
                CompanyName = _companyName,
                ProductName = _productName,
                PackageName = _packageName,
                AssemblyName = _assemblyName,
                NamespaceName = _namespaceName,
                Description = _description,
                Author = _author,
                Version = _version,
                SelectedLicenseType = (int)_selectedLicenseType,
                TargetLocation = _targetLocation,
                CreateDocsFolder = createDocsFolder,
                CreateSamplesFolder = createSamplesFolder,
                CreateEditorFolder = createEditorFolder,
                CreateTestsFolder = createTestsFolder,
                Dependencies = CloneDependencies(_dependencies)
            };

            EditorUserSettings.SetConfigValue(PreferencesKey, JsonUtility.ToJson(preferences));
        }

        private static List<PackageDependency> CloneDependencies(List<PackageDependency> dependencies) {
            var clone = new List<PackageDependency>(dependencies.Count);
            foreach (var dependency in dependencies) {
                clone.Add(new PackageDependency {
                    PackageName = dependency.PackageName,
                    Version = dependency.Version
                });
            }

            return clone;
        }

        /// <summary>
        /// Creates the package repository skeleton, optional folders, and the companion Unity project.
        /// </summary>
        private void CreatePackageScaffolding() {
            Directory.CreateDirectory(RootDirectory);
            Directory.CreateDirectory(PackageDirectory);
            Directory.CreateDirectory(ProjectDirectory);

            if (createDocsFolder) {
                CreateDocsFolder(RootDirectory);
            }

            CreatePackageFolders();
            CreateProjectStructure();
            CreateRootFiles();

            GitUtility.InitializeRepository(RootDirectory, _packageName);

            Debug.Log($"Package scaffolding created successfully at {RootDirectory}");
        }

        /// <summary>
        /// Creates the package folders that live inside the generated package root.
        /// </summary>
        private void CreatePackageFolders() {
            CreateRuntimeFolder();

            if (createSamplesFolder) {
                CreateSamplesFolder();
            }

            if (createEditorFolder) {
                CreateEditorFolder();
            }

            if (createTestsFolder) {
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

            PlayerSettings.companyName = _companyName;
            PlayerSettings.productName = _productName;
            PlayerSettings.bundleVersion = _version;
            foreach (var target in NamedTargets) {
                PlayerSettings.SetApplicationIdentifier(target, _packageName);
            }

            EditorSettings.projectGenerationRootNamespace = _namespaceName;

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
            CreateFile(changelogPath, ChangelogTemplate.GetContent(_version));
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
            string asmDefPath = Path.Combine(path, $"{_assemblyName}.asmdef");
            CreateFile(asmDefPath, GetRuntimeAsmDef());
        }

        /// <summary>
        /// Creates the samples assembly definition in the provided folder.
        /// </summary>
        private void CreateSamplesAsmDef(string path) {
            string asmDefPath = Path.Combine(path, $"{_assemblyName}.asmdef");
            CreateFile(asmDefPath, GetSamplesAsmDef());
        }

        /// <summary>
        /// Creates the editor-only assembly definition in the provided folder.
        /// </summary>
        private void CreateEditorAsmDef(string path) {
            string asmDefPath = Path.Combine(path, $"{_assemblyName}.Editor.asmdef");
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
