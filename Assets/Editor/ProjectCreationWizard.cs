using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;

public class ProjectCreationWizard : EditorWindow {

    internal string companyName = "Doji Technologies";
    internal string productName = "MyProject";
    internal string version = "1.0.0";
    internal string targetLocation = "../";
    private bool autoOpenAfterCreation = true;

    private string projectDirectory { get { return Path.Combine(targetLocation, productName); } }

    private static string originalCompanyName;
    private static string originalProductName;
    private static string originalVersion;
    private static List<string> originalIdentifiers = new List<string>();
    private static List<BuildTargetGroup> namedTargets = new List<BuildTargetGroup> {
        BuildTargetGroup.Standalone,
        BuildTargetGroup.Android
    };
    private static string rootNamespace;

    [MenuItem("Tools/Project Creation Wizard")]
    public static void ShowWindow() {
        GetWindow<ProjectCreationWizard>("Project Creation Wizard");
    }

    private void OnGUI() {
        GUILayout.Space(10);
        GUILayout.Label("Project Information", EditorStyles.boldLabel);

        companyName = EditorGUILayout.TextField("Company Name", companyName);
        productName = EditorGUILayout.TextField("Product Name", productName);
        version = EditorGUILayout.TextField("Version", version);
        targetLocation = EditorGUILayout.TextField("Target Location", targetLocation);

        autoOpenAfterCreation = EditorGUILayout.Toggle("Auto-Open After Creation", autoOpenAfterCreation);

        GUILayout.Space(10);
        if (GUILayout.Button("Create Project")) {
            CreateProjectStructure();
        }
    }

    private void CreateProjectStructure() {
        Directory.CreateDirectory(projectDirectory);

        UpdateProjectSettings();

        Directory.CreateDirectory(Path.Combine(projectDirectory, "Assets"));
        CopyDirectory("Packages", Path.Combine(projectDirectory, "Packages"));
        CopyDirectory("ProjectSettings", Path.Combine(projectDirectory, "ProjectSettings"));

        RevertProjectSettings();

        string gitignorePath = Path.Combine(Directory.GetCurrentDirectory(), ".gitignore");
        if (File.Exists(gitignorePath)) {
            string targetPath = Path.Combine(projectDirectory, ".gitignore");
            CopyFile(gitignorePath, targetPath);
        }

        Debug.Log($"Project created successfully at {projectDirectory}");

        if (autoOpenAfterCreation) {
            OpenProjectInUnity(projectDirectory);
        }
    }

    private void UpdateProjectSettings() {
        originalCompanyName = PlayerSettings.companyName;
        originalProductName = PlayerSettings.productName;
        originalVersion = PlayerSettings.bundleVersion;
        originalIdentifiers.Clear();
        foreach (BuildTargetGroup target in namedTargets) {
            originalIdentifiers.Add(PlayerSettings.GetApplicationIdentifier(target));
        }
        rootNamespace = EditorSettings.projectGenerationRootNamespace;

        PlayerSettings.companyName = companyName;
        PlayerSettings.productName = productName;
        PlayerSettings.bundleVersion = version;
        foreach (BuildTargetGroup target in namedTargets) {
            PlayerSettings.SetApplicationIdentifier(target, $"com.{companyName.ToLower().Replace(" ", "")}.{productName.ToLower()}");
        }
        EditorSettings.projectGenerationRootNamespace = productName;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void RevertProjectSettings() {
        PlayerSettings.companyName = originalCompanyName;
        PlayerSettings.productName = originalProductName;
        PlayerSettings.bundleVersion = originalVersion;
        for (int i = 0; i < originalIdentifiers.Count; i++) {
            PlayerSettings.SetApplicationIdentifier(namedTargets[i], originalIdentifiers[i]);
        }
        EditorSettings.projectGenerationRootNamespace = rootNamespace;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

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

    private void CopyFile(string sourceFileName, string destFileName) {
        if (!File.Exists(destFileName)) {
            File.Copy(sourceFileName, destFileName, overwrite: false);
        }
    }

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
