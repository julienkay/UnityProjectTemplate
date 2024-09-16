using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditorInternal;
using System;

public partial class PackageScaffolder : EditorWindow {

    internal string companyName = "Doji Technologies";
    internal string productName = "MyPackage";
    internal string packageName = "com.doji.package";
    internal string assemblyName = "Doji.Package";
    internal string namespaceName = "Doji.AI.PackageNS";
    internal string description = "A short description for readmes etc";
    internal string author = "Julien Kipp";
    internal string version = "1.0.0";
    internal LicenseType licenseType = LicenseType.MIT;

    internal string targetLocation = "../";
    internal string rootDirectory { get { return Path.Combine(targetLocation, packageName); } }
    internal string packageDirectory { get { return Path.Combine(rootDirectory, packageName); } }
    internal string projectDirectory { get { return Path.Combine(rootDirectory, "projects", productName); } }

    // New toggles for Documentation and Tests folders
    private bool createDocsFolder = true;
    private bool createSamplesFolder = false;
    private bool createEditorFolder = false;
    private bool createTestsFolder = false;

    [SerializeField]
    internal List<PackageDependency> dependencies = new List<PackageDependency> { new() { packageName = "com.unity.sentis", version = "2.0.0" } };
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
        packageName = EditorGUILayout.TextField("Identifier", packageName);
        assemblyName = EditorGUILayout.TextField("Assembly Name", assemblyName);
        namespaceName = EditorGUILayout.TextField("Namespace", namespaceName);
        description = EditorGUILayout.TextField("Description", description);
        author = EditorGUILayout.TextField("Author", author);
        version = EditorGUILayout.TextField("Version", version);

        licenseType = (LicenseType)EditorGUILayout.EnumPopup("License Type", licenseType);

        // Add toggles for optional folders
        createDocsFolder = EditorGUILayout.Toggle("Create Documentation Folder", createDocsFolder);
        createSamplesFolder = EditorGUILayout.Toggle("Create Samples Folder", createSamplesFolder);
        createEditorFolder = EditorGUILayout.Toggle("Create Editor Folder", createEditorFolder);
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
        // Create the root structure
        Directory.CreateDirectory(rootDirectory);
        Directory.CreateDirectory(packageDirectory);
        Directory.CreateDirectory(projectDirectory);

        if (createDocsFolder) {
            CreateDocsFolder(rootDirectory);
        }

        // Create the subfolders inside the second 'com.company.package' (or identifier) folder
        CreatePackageFolders();

        // Create the project folder structure
        CreateProjectStructure();

        // Create the LICENSE and README.md files
        CreateRootFiles();

        GitUtility.InitializeRepository(rootDirectory, packageName);

        Debug.Log($"Package scaffolding created successfully at {rootDirectory}");
    }

    private void CreatePackageFolders() {
        string runtimePath = Path.Combine(packageDirectory, "Runtime");
        Directory.CreateDirectory(runtimePath);
        CreateRuntimeAsmDef(runtimePath);
        CreateAssemblyInfo(runtimePath);

        // Conditionally create the Samples, Editor and Tests folders
        if (createSamplesFolder) {
            CreateSamplesFolder();
        }

        if (createEditorFolder) {
            Directory.CreateDirectory(Path.Combine(packageDirectory, "Editor"));
        }

        if (createTestsFolder) {
            Directory.CreateDirectory(Path.Combine(packageDirectory, "Tests"));
        }

        // Create the package files inside this path
        CreatePackageFiles(packageDirectory);
    }

    private void CreateProjectStructure() {
        UpdateProjectSettings();

        Directory.CreateDirectory(Path.Combine(projectDirectory, "Assets"));

        // Copy existing Unity project folders and .gitignore to the project folder
        CopyDirectory("Packages", Path.Combine(projectDirectory, "Packages"));
        CopyDirectory("ProjectSettings", Path.Combine(projectDirectory, "ProjectSettings"));

        RevertProjectSettings();

        // Copy the .gitignore file
        string gitignorePath = Path.Combine(Directory.GetCurrentDirectory(), ".gitignore");
        if (File.Exists(gitignorePath)) {
            string targetPath = Path.Combine(projectDirectory, ".gitignore");
            CopyFile(gitignorePath, targetPath);
        }

        // add the package to project manifest via local (relative) path
        string projectManifestPath = Path.Combine(projectDirectory, "Packages", "manifest.json");
        PackageManagerHelper.InsertPackageLine(projectManifestPath, packageName);
    }

    private static string originalCompanyName;
    private static string originalProductName;
    private static string originalVersion;
    private static List<string> originalIdentifiers = new List<string>();
    private static List<BuildTargetGroup> namedTargets = new List<BuildTargetGroup> {
        BuildTargetGroup.Standalone,
        BuildTargetGroup.Android
    };
    private static string rootNamespace;

    private void UpdateProjectSettings() {
        // Store the original values
        originalCompanyName = PlayerSettings.companyName;
        originalProductName = PlayerSettings.productName;
        originalVersion = PlayerSettings.bundleVersion;
        originalIdentifiers.Clear();
        foreach (BuildTargetGroup target in namedTargets) {
            originalIdentifiers.Add(PlayerSettings.GetApplicationIdentifier(target));
        }
        rootNamespace = EditorSettings.projectGenerationRootNamespace;

        // Set the new values
        PlayerSettings.companyName = companyName;
        PlayerSettings.productName = productName;
        PlayerSettings.bundleVersion = version;
        foreach (BuildTargetGroup target in namedTargets) {
            PlayerSettings.SetApplicationIdentifier(target, packageName);
        }
        EditorSettings.projectGenerationRootNamespace = namespaceName;

        // Refresh the editor to apply changes
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void RevertProjectSettings() {
        // Revert to the original values
        PlayerSettings.companyName = originalCompanyName;
        PlayerSettings.productName = originalProductName;
        PlayerSettings.bundleVersion = originalVersion;
        for (int i = 0; i < originalIdentifiers.Count; i++) {
            PlayerSettings.SetApplicationIdentifier(namedTargets[i], originalIdentifiers[i]);
        }
        EditorSettings.projectGenerationRootNamespace = rootNamespace;

        // Save and refresh
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void CreateRootFiles() {
        // Create a LICENSE file in the root
        string licensePath = Path.Combine(rootDirectory, "LICENSE");
        CreateFile(licensePath, GetLicense());

        // Create a README.md file in the root
        string readmePath = Path.Combine(rootDirectory, "README.md");
        CreateFile(readmePath, GetRepositoryReadme());
    }

    private void CreatePackageFiles(string path) {
        // Create package.json manifest file inside the identifier folder
        string packageManifestPath = Path.Combine(path, "package.json");
        // package.json is one of the few files getting overwritten in case we added samples, dependencies or anything
        // TODO: make this an UpdateFile, that does not overwrite existing entries, only adds new ones
        CreateFile(packageManifestPath, GetPackageManifest(), overwrite: true);

        // Create README.md file inside the identifier folder
        string readmePath = Path.Combine(path, "README.md");
        CreateFile(readmePath, GetPackageReadme());

        // Create CHANGELOG.md file inside the identifier folder
        string changelogPath = Path.Combine(path, "CHANGELOG.md");
        CreateFile(changelogPath, ChangelogTemplate.GetContent(version));
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
            CopyFile(file, destFile);
        }

        // Copy all subdirectories recursively
        foreach (string subDir in Directory.GetDirectories(sourceDir)) {
            string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
            CopyDirectory(subDir, destSubDir);
        }
    }

    // Utility method to create .asmdef for the 'Runtime' folder
    private void CreateRuntimeAsmDef(string path) {
        string asmDefPath = Path.Combine(path, $"{assemblyName}.asmdef");
        CreateFile(asmDefPath, GetRuntimeAsmDef());
    }

    // Utility method to create .asmdef for the 'Samples' folder
    private void CreateSamplesAsmDef(string path) {
        string asmDefPath = Path.Combine(path, $"{assemblyName}.asmdef");
        CreateFile(asmDefPath, GetSamplesAsmDef());
    }

    // Utility method to create AssemblyInfo.cs files
    private void CreateAssemblyInfo(string path) {
        string asmDefPath = Path.Combine(path, "AssemblyInfo.cs");
        CreateFile(asmDefPath, GetAssemblyInfo());
    }

    private void CreateSamplesFolder() {
        string samplesPath = Path.Combine(packageDirectory, "Samples~");
        Directory.CreateDirectory(samplesPath);

        Directory.CreateDirectory(Path.Combine(samplesPath, "00-SharedSampleAssets"));
        Directory.CreateDirectory(Path.Combine(samplesPath, "01-BasicSample"));
        CreateSamplesAsmDef(samplesPath);

        CreateFile(Path.Combine(samplesPath, "01-BasicSample", "BasicSample.cs"), GetSampleScript());
    }

    private void CreateDocsFolder(string path) {
        path = Path.Combine(path, "docs");
        Directory.CreateDirectory(path);
        CreateDocfxFolders(path);
        CreateDocfxFiles(path);
    }

    private void CreateDocfxFolders(string path) {
        // Copy docs folder potentially including some standard files
        CopyDirectory("docs", Path.Combine(path));
    }

    private void CreateDocfxFiles(string path) {
        // Create docfx.json in the docs folder
        string docfxConfigPath = $"{path}/docfx.json";
        CreateFile(docfxConfigPath, GetDocfxJson());

        // Create docfx-pdf.json in the docs folder
        string docfxPdfConfigPath = $"{path}/docfx-pdf.json";
        CreateFile(docfxPdfConfigPath, GetDocfxPdfJson());

        // Create filterConfig.yml in the docs folder
        string filterConfigPath = $"{path}/filterConfig.yml";
        CreateFile(filterConfigPath, GetFilterConfig());

        // Create root index.md in the docs folder
        string indexPath = $"{path}/index.md";
        CreateFile(indexPath, GetIndexMD());

        string tocPath = $"{path}/toc.yml";
        CreateFile(tocPath, GetRootToc());

        string manualTocPath = $"{path}/manual/toc.yml";
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
