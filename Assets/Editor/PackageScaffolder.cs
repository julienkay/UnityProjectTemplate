using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditorInternal;
using System;

public class PackageScaffolder : EditorWindow {

    private string companyName = "Doji Technologies";
    private string productName = "NewPackage";
    private string identifier = "com.doji.package";
    private string assemblyName = "Doji.Package";
    private string description = "A short description for readmes etc";
    private string author = "Julien Kipp";
    private string version = "1.0.0";
    private LicenseTemplate.LicenseType selectedLicense = LicenseTemplate.LicenseType.MIT;

    // New toggles for Documentation and Tests folders
    private bool createEditorFolder = false;
    private bool createDocumentationFolder = false;
    private bool createTestsFolder = false;

    [SerializeField]
    private List<PackageDependency> dependencies = new List<PackageDependency> { new() { packageName = "com.unity.sentis", version = "2.0.0" } };
    [SerializeField]
    private ReorderableList m_DependenciesList;

    private static class Styles {
        public static readonly GUIContent version = EditorGUIUtility.TrTextContent("Version", "Must follow SemVer (ex: 1.0.0-preview.1).");
        public static readonly GUIContent package = EditorGUIUtility.TrTextContent("Package name", "Must be lowercase");
    }

    private SerializedObject serializedObject;

    [Serializable]
    public class PackageDependency {
        public string packageName;
        public string version;
    }

    [MenuItem("Tools/Package Creation Wizard")]
    public static void ShowWindow() {
        GetWindow<PackageScaffolder>("Package Creation Wizard");
    }

    private void OnEnable() {
        serializedObject = new SerializedObject(this);

        // Initialize the reorderable list
        m_DependenciesList = new ReorderableList(serializedObject, serializedObject.FindProperty("dependencies"), true, false, true, true) {
            drawElementCallback = DrawDependencyListElement,
            drawHeaderCallback = DrawDependencyHeaderElement,
            elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing
        };
    }

    private void DrawDependencyHeaderElement(Rect rect) {
        var w = rect.width;
        rect.x += 4;
        rect.width = w / 3 * 2 - 2;
        GUI.Label(rect, Styles.package, EditorStyles.label);

        rect.x += w / 3 * 2;
        rect.width = w / 3 - 4;
        GUI.Label(rect, Styles.version, EditorStyles.label);
    }

    private void DrawDependencyListElement(Rect rect, int index, bool isActive, bool isFocused) {
        var list = m_DependenciesList.serializedProperty;
        var dependency = list.GetArrayElementAtIndex(index);
        var packageName = dependency.FindPropertyRelative("packageName");
        var version = dependency.FindPropertyRelative("version");

        var w = rect.width;
        rect.x += 4;
        rect.width = w / 3 * 2 - 2;

        rect.height -= EditorGUIUtility.standardVerticalSpacing;
        packageName.stringValue = EditorGUI.TextField(rect, packageName.stringValue);

        using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(packageName.stringValue))) {
            rect.x += w / 3 * 2;
            rect.width = w / 3 - 4;
            version.stringValue = EditorGUI.TextField(rect, version.stringValue);

            //if (!string.IsNullOrWhiteSpace(version.stringValue))
            //    ValidateVersion(packageName.stringValue, version.stringValue, errorMessages, warningMessages);
        }
    }

    private void OnGUI() {
        GUILayout.Label("Package Creation Wizard", EditorStyles.boldLabel);

        companyName = EditorGUILayout.TextField("Company Name", companyName);
        productName = EditorGUILayout.TextField("Product Name", productName);
        identifier = EditorGUILayout.TextField("Identifier", identifier);
        assemblyName = EditorGUILayout.TextField("Assembly Name", assemblyName);
        description = EditorGUILayout.TextField("Description", description);
        author = EditorGUILayout.TextField("Author", author);
        version = EditorGUILayout.TextField("Version", version);

        selectedLicense = (LicenseTemplate.LicenseType)EditorGUILayout.EnumPopup("License Type", selectedLicense);

        // Add toggles for optional folders
        createEditorFolder = EditorGUILayout.Toggle("Create Editor Folder", createEditorFolder);
        createDocumentationFolder = EditorGUILayout.Toggle("Create Documentation Folder", createDocumentationFolder);
        createTestsFolder = EditorGUILayout.Toggle("Create Tests Folder", createTestsFolder);

        // Draw the reorderable list
        GUILayout.Label("Dependencies (Optional)", EditorStyles.boldLabel);
        m_DependenciesList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Create Package")) {
            CreatePackageScaffolding();
        }
    }

    private void CreatePackageScaffolding() {
        // Define the root path for the package using the identifier
        string rootPath = $"../{identifier}";

        if (Directory.Exists(rootPath)) {
            Debug.LogError($"The directory {rootPath} already exists!");
            return;
        }

        // Create the root structure
        Directory.CreateDirectory(rootPath);
        Directory.CreateDirectory($"{rootPath}/{identifier}");
        Directory.CreateDirectory($"{rootPath}/projects");

        // Create the subfolders inside the second 'com.doji.package' (or identifier) folder
        CreateSubfolders($"{rootPath}/{identifier}");

        // Create the project folder structure
        CreateProjectStructure(rootPath);

        // Create the LICENSE and README.md files
        CreateRootFiles(rootPath);

        GitUtility.InitializeRepository(rootPath, identifier);

        Debug.Log($"Package scaffolding created successfully at {rootPath}");
    }

    private void CreateSubfolders(string path) {
        Directory.CreateDirectory($"{path}/Runtime");
        CreateAsmDef($"{path}/Runtime");
        CreateAssemblyInfo($"{path}/Runtime");

        // Conditionally create the Editor, Documentation and Tests folders
        if (createEditorFolder) {
            Directory.CreateDirectory($"{path}/Editor");
        }

        if (createDocumentationFolder) {
            Directory.CreateDirectory($"{path}/Documentation");
        }

        if (createTestsFolder) {
            Directory.CreateDirectory($"{path}/Tests");
        }

        // Create the package files inside this path
        CreatePackageFiles(path);
    }

    private void CreateProjectStructure(string rootPath) {
        string projectPath = $"{rootPath}/projects/{productName}";

        UpdateProjectSettings();

        // Create the Product Name folder inside the projects folder
        Directory.CreateDirectory(projectPath);
        Directory.CreateDirectory($"{projectPath}/Assets");

        // Copy existing Unity project folders and .gitignore to the project folder
        CopyDirectory("Packages", $"{projectPath}/Packages");
        CopyDirectory("ProjectSettings", $"{projectPath}/ProjectSettings");

        RevertProjectSettings();

        // Copy the .gitignore file
        string gitignorePath = Path.Combine(Directory.GetCurrentDirectory(), ".gitignore");
        if (File.Exists(gitignorePath)) {
            File.Copy(gitignorePath, $"{projectPath}/.gitignore", overwrite: true);
        }

        // add the package to project manifest via local (relative) path
        string projectManifestPath = $"{projectPath}/Packages/manifest.json";
        PackageManagerHelper.InsertPackageLine(projectManifestPath, identifier);
    }

    private static string originalCompanyName;
    private static string originalProductName;
    private static string originalVersion;
    private static string originalIdentifier;

    private void UpdateProjectSettings() {
        // Store the original values
        originalCompanyName = PlayerSettings.companyName;
        originalProductName = PlayerSettings.productName;
        originalVersion = PlayerSettings.bundleVersion;
        originalIdentifier = PlayerSettings.applicationIdentifier;

        // Set the new values
        PlayerSettings.companyName = companyName;
        PlayerSettings.productName = productName;
        PlayerSettings.bundleVersion = version;
        PlayerSettings.applicationIdentifier = identifier;

        // Refresh the editor to apply changes
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void RevertProjectSettings() {
        // Revert to the original values
        PlayerSettings.companyName = originalCompanyName;
        PlayerSettings.productName = originalProductName;
        PlayerSettings.bundleVersion = originalVersion;
        PlayerSettings.applicationIdentifier = originalIdentifier;

        // Save and refresh
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void CreateRootFiles(string rootPath) {
        // Create a LICENSE file in the root
        string licensePath = $"{rootPath}/LICENSE";
        File.WriteAllText(licensePath, selectedLicense.GetContent(author));

        // Create a README.md file in the root
        string readmePath = $"{rootPath}/README.md";
        File.WriteAllText(readmePath, ReadmeTemplate.GetRepositoryReadme(productName, identifier, description));
    }

    private void CreatePackageFiles(string path) {
        // Create package.json manifest file inside the identifier folder
        string packageManifestPath = $"{path}/package.json";
        File.WriteAllText(packageManifestPath, PackageManifestTemplate.GetContent(productName, identifier, description, companyName, version, dependencies));

        // Create README.md file inside the identifier folder
        string readmePath = $"{path}/README.md";
        File.WriteAllText(readmePath, ReadmeTemplate.GetPackageReadme(identifier, description));

        // Create CHANGELOG.md file inside the identifier folder
        string changelogPath = $"{path}/CHANGELOG.md";
        File.WriteAllText(changelogPath, ChangelogTemplate.GetContent(version));
    }

    // Utility method to copy directories
    private void CopyDirectory(string sourceDir, string destinationDir) {
        // Check if source directory exists
        if (!Directory.Exists(sourceDir)) {
            Debug.LogWarning($"Source directory {sourceDir} does not exist. Skipping copy.");
            return;
        }

        // Create destination directory if it doesn't exist
        Directory.CreateDirectory(destinationDir);

        // Copy all files
        foreach (string file in Directory.GetFiles(sourceDir)) {
            string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }

        // Copy all subdirectories
        foreach (string subDir in Directory.GetDirectories(sourceDir)) {
            string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
            CopyDirectory(subDir, destSubDir);
        }
    }

    // Utility method to create .asmdef files
    private void CreateAsmDef(string path) {
        string asmDefPath = $"{path}/{assemblyName}.asmdef";
        File.WriteAllText(asmDefPath, AsmDefTemplate.GetContent(assemblyName));
    }

    // Utility method to create AssemblyInfo.cs files
    private void CreateAssemblyInfo(string path) {
        string asmDefPath = $"{path}/AssemblyInfo.cs";
        File.WriteAllText(asmDefPath, AssemblyInfoTemplate.GetContent(assemblyName, companyName, author, description, version));
    }
}
