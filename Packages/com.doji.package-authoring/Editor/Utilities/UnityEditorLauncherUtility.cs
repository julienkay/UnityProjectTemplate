using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace Doji.PackageAuthoring.Editor.Utilities {
    /// <summary>
    /// Opens generated projects in the same Unity editor version that is currently running the wizard.
    /// </summary>
    internal static class UnityEditorLauncherUtility {
        /// <summary>
        /// Attempts to open the provided project path in the current Unity editor installation.
        /// </summary>
        /// <param name="projectPath">Absolute path to the Unity project that should be opened.</param>
        /// <returns><see langword="true"/> when a launch command was started successfully.</returns>
        public static bool TryOpenProjectInCurrentEditor(string projectPath) {
            if (string.IsNullOrWhiteSpace(projectPath)) {
                UnityEngine.Debug.LogError("Cannot open a Unity project because the project path is empty.");
                return false;
            }

#if UNITY_EDITOR_OSX
            return TryOpenProjectOnMac(projectPath);
#elif UNITY_EDITOR_WIN
            return TryOpenProjectWithExecutable(EditorApplication.applicationPath, projectPath);
#else
            return TryOpenProjectWithExecutable(ResolveEditorExecutablePath(), projectPath);
#endif
        }

        /// <summary>
        /// Resolves the running Unity app bundle to its launchable executable on macOS and starts it with
        /// a <c>-projectPath</c> argument.
        /// </summary>
        private static bool TryOpenProjectOnMac(string projectPath) {
            string applicationPath = EditorApplication.applicationPath;
            string editorExecutablePath = ResolveMacEditorExecutablePath(applicationPath);
            UnityEngine.Debug.Log(
                $"Resolving macOS Unity editor launch path. applicationPath='{applicationPath}', executablePath='{editorExecutablePath}', projectPath='{projectPath}'.");
            return TryOpenProjectWithExecutable(editorExecutablePath, projectPath);
        }

        /// <summary>
        /// Starts the Unity editor executable directly with a <c>-projectPath</c> argument.
        /// </summary>
        private static bool TryOpenProjectWithExecutable(string editorExecutablePath, string projectPath) {
            if (string.IsNullOrWhiteSpace(editorExecutablePath) || !File.Exists(editorExecutablePath)) {
                UnityEngine.Debug.LogError("Could not locate Unity Editor executable to open new project.");
                return false;
            }

            Process.Start(new ProcessStartInfo {
                FileName = editorExecutablePath,
                Arguments = $"-projectPath {Quote(projectPath)}",
                UseShellExecute = false
            });

            UnityEngine.Debug.Log($"Opening project in Unity: {projectPath}");
            return true;
        }

        /// <summary>
        /// Resolves the Unity editor executable when the platform does not expose a directly launchable path by default.
        /// </summary>
        /// <remarks>
        /// Windows uses <see cref="EditorApplication.applicationPath"/> directly. macOS launches the outer
        /// <c>.app</c> bundle via <c>open</c>. This fallback is kept only for other editor platforms where Unity
        /// may expose an installation contents path but not the final executable path.
        /// </remarks>
        /// <returns>
        /// The absolute path to the Unity editor executable, or <see cref="string.Empty"/> when it cannot be resolved.
        /// </returns>
        private static string ResolveEditorExecutablePath() {
            // Unity's contents path points at the installation's internal data root rather than the launchable app path.
            string contentsPath = EditorApplication.applicationContentsPath;
            if (Directory.Exists(contentsPath)) {
                string linuxExecutablePath = Path.Combine(contentsPath, "Linux", "Unity");
                if (File.Exists(linuxExecutablePath)) {
                    return linuxExecutablePath;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Resolves the real Unity executable inside a macOS app bundle or contents directory.
        /// </summary>
        /// <param name="applicationPath">The value reported by <see cref="EditorApplication.applicationPath"/>.</param>
        /// <returns>The executable path inside the bundle, or <see cref="string.Empty"/> when it cannot be found.</returns>
        private static string ResolveMacEditorExecutablePath(string applicationPath) {
            if (File.Exists(applicationPath)) {
                return applicationPath;
            }

            if (Directory.Exists(applicationPath) && applicationPath.EndsWith(".app")) {
                string bundleExecutablePath = Path.Combine(applicationPath, "Contents", "MacOS", "Unity");
                if (File.Exists(bundleExecutablePath)) {
                    return bundleExecutablePath;
                }
            }

            string contentsPath = EditorApplication.applicationContentsPath;
            if (Directory.Exists(contentsPath)) {
                string contentsExecutablePath = Path.Combine(contentsPath, "MacOS", "Unity");
                if (File.Exists(contentsExecutablePath)) {
                    return contentsExecutablePath;
                }
            }

            return string.Empty;
        }

        private static string Quote(string value) {
            return $"\"{value.Replace("\"", "\\\"")}\"";
        }
    }
}
